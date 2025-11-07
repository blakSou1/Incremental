using System.Linq;
using UnityEngine;
using Cysharp.Threading.Tasks;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class MovableObject : MonoBehaviour
{
    [HideInInspector] public SpriteRenderer visual;
    private BoxCollider2D boxCollider;
    [HideInInspector] public Rigidbody2D body;
    private MoveEngine _moveEngine;

    private float gravity = 2.5f;

    private bool isMoving;
    protected bool isDragging;

    [HideInInspector] public BatterySlot mySlot;

    private void Awake()
    {
        visual = GetComponentInChildren<SpriteRenderer>();
        visual.material = CMS.GetAll<CMSEntity>().FirstOrDefault(x => x.Is<ConfigItemsInPipe>())!
            .Get<ConfigItemsInPipe>().baseAllShader;
        boxCollider = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();

        _moveEngine = new MoveEngine(transform, visual.transform);
        body.gravityScale = gravity;
        body.linearVelocityY = -5f;
    }

    private void Update()
    {
        HandleRightClickDrag().Forget();
    }

    private void OnCollisionStay2D(Collision2D other)
    {
        if (!isDragging && isMoving) _ = MoveTo(transform.position);
    }

    private async UniTaskVoid HandleRightClickDrag()
    {
        if (G.inputs.Player.Attack.WasPressedThisFrame())
        {
            RaycastHit2D[] hits = Physics2D.RaycastAll(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), Vector2.zero);
            RaycastHit2D hit = hits.FirstOrDefault(x => x.collider.transform.GetComponent<MovableObject>());
            if (hit.collider && hit.collider.gameObject == gameObject)
            {
                isDragging = true;
                body.gravityScale = 0;
                visual.sortingOrder = 100;
                ChangeRB(true);
                if(mySlot) mySlot.RemoveBattery();
            }
        }
        
        if (G.inputs.Player.Attack.IsPressed() && isDragging)
        {
            Vector3 targetPosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()); 
            MoveTo(targetPosition, 0).Forget();
        }
        
        if (G.inputs.Player.Attack.WasReleasedThisFrame() && isDragging)
        {
            CheckAndPut();
            if (!isMoving) body.gravityScale = gravity;
            
            isDragging = false;
        }
    }

    private void CheckAndPut()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue()), 0.5f);

        foreach (var hit in hits)
        {
            var slot = hit.GetComponent<BatterySlot>();
            if (slot != null && !slot.isClosed)
            {
                ChangeRB(false);
                slot.SetBattery(this);
                mySlot = slot;
                _ = MoveTo(slot.GetMyPos());
                return;
            }
        }
        visual.sortingOrder = 10;
    }

    public async UniTask MoveTo(Vector3 target, float targetRotation = 0)
    {
        _moveEngine.SetTarget(target, targetRotation);
        body.gravityScale = 0;
        body.linearVelocity = Vector2.zero;
        if (isMoving)
        {
            while (isMoving) await UniTask.Yield();
            return;
        }
        isMoving = true;
        while (_moveEngine.CheckDistant())
        {
            _moveEngine.SmoothFollow();
            _moveEngine.FollowRotation();
            await UniTask.Yield();
        }
        _moveEngine.SetToTargetPos();
        isMoving = false;
        if(!isDragging) body.gravityScale = gravity;
    }
    public void ChangeRB(bool turnOn)
    {
        body.bodyType = turnOn ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
        boxCollider.isTrigger = !turnOn;
    }

    public class MoveEngine
    {
        private Transform _cardTransform;
        private Transform _visualTransform;
        
        private Vector2 targetPosition;
        private float targetRotation;
        private Vector2 velocity; 
        private float smoothTime = 0.12f; 
        private float maxVelocity = 8.5f; 
        private Vector2 currentVelocity;
        private Vector3 movementDelta;
        private Vector3 rotationDelta;

        public MoveEngine(Transform t, Transform visual)
        {
            _cardTransform = t;
            _visualTransform = visual;
        }

        public void SetTarget(Vector2 targetPos, float targetRot)
        {
            targetPosition = targetPos;
            targetRotation = targetRot;
            maxVelocity = 27;
        }

        public bool CheckDistant()
        {
            if (_cardTransform == null) return false;
            return Vector2.Distance(_cardTransform.position, targetPosition) > 0.01f || velocity.magnitude > 0.01f;
        }
            
        public void SmoothFollow()
        {
            Vector2 newPosition = Vector2.SmoothDamp(_cardTransform.position, targetPosition, ref currentVelocity, smoothTime, maxVelocity, Time.deltaTime);
            velocity = (newPosition - (Vector2)_cardTransform.position) / Time.deltaTime;

            if (velocity.sqrMagnitude > maxVelocity * maxVelocity)
            {
                velocity = velocity.normalized * maxVelocity;
            }

            Vector2 newPos = newPosition + velocity * (Time.deltaTime);
            _cardTransform.GetComponent<Rigidbody2D>().MovePosition(new Vector3(newPos.x, newPos.y, _cardTransform.position.z));
        } 
        
        public void FollowRotation()
        {
            float rotationAmount = 20;
            float rotationSpeed = 20;

            Vector3 movement = _cardTransform.position - (Vector3)targetPosition;
            movementDelta = Vector3.Lerp(movementDelta, movement, 25 * Time.deltaTime);
            Vector3 movementRotation = movement * rotationAmount;
            rotationDelta = Vector3.Lerp(rotationDelta , movementRotation, rotationSpeed * Time.deltaTime);
            _visualTransform.eulerAngles = new Vector3(_visualTransform.eulerAngles.x, _visualTransform.eulerAngles.y, Mathf.Clamp(rotationDelta.x, -60, 60) + targetRotation);
        }
        
        public void SetToTargetPos()
        {   
            _cardTransform.position = new Vector3(targetPosition.x, targetPosition.y, _cardTransform.position.z);
            _visualTransform.localEulerAngles = new Vector3(0, 0, this.targetRotation);
            velocity = Vector2.zero;
        }
    }
}