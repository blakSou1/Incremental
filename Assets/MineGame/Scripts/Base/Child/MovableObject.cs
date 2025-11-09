using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(BoxCollider2D))]
public class MovableObject : MonoBehaviour
{
    private BoxCollider2D boxCollider;
    [HideInInspector] public Rigidbody2D body;

    [HideInInspector] public float gravity = 2.5f;

    private void Awake()
    {
        boxCollider = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();

        body.gravityScale = gravity;
        body.linearVelocityY = -5f;
    }

    public void StartDragging()
    {
        ChangeRB(true);
    }

    public void CheckAndPut()
    {
        body.gravityScale = gravity;
        StopAllCoroutines();
    }

    public IEnumerator MoveTo()
    {
        body.gravityScale = 0;

        while (true)
        {
            body.linearVelocity = Vector2.zero;
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());

            mousePosition.z = transform.position.z;

            transform.position = Vector3.Lerp(transform.position, mousePosition, 1);

            yield return new WaitForFixedUpdate();
        }
    }

    public void ChangeRB(bool turnOn)
    {
        body.bodyType = turnOn ? RigidbodyType2D.Dynamic : RigidbodyType2D.Kinematic;
        boxCollider.isTrigger = !turnOn;
    }
}
