using UnityEngine;

public delegate void PointerEvent<T>(T data);

public class ControlSystem : MonoBehaviour
{
    [SerializeField] private LayerMask mask;

    private PointerViewController Pointer;

    public static ControlSystem Instance;

    private GameObject HoveredObjects;
    private ReactToPointer PrimaryHoveredObject;
    private float time;

    public PointerEvent<string> PointerTooltipUpdated;

    private void Start()
    {
        Instance = this;
        Pointer = FindFirstObjectByType<PointerViewController>();
    }

    /// <summary>
    /// помещает сообщение в тултип и отслеживает активность панели тултипа
    /// </summary>
    /// <param name="msg"></param>
    public void OnPointerTooltipUpdated(string msg) =>
        PointerTooltipUpdated?.Invoke(msg);

    private void FixedUpdate()
    {
        HoveredObjects = null;

        Vector2 pos = G.inputs.Player.mousePosition.ReadValue<Vector2>();
        Pointer.UpdatePosition(pos);

        Ray ray = Camera.main.ScreenPointToRay(pos);
        RaycastHit hit;

        Pointer.UpdatePosition(pos);

        if (Physics.Raycast(ray, out hit, 1000, mask))
        {
            HoveredObjects = hit.transform.gameObject;

            if (PrimaryHoveredObject == null || HoveredObjects != PrimaryHoveredObject.gameObject)
            {
                PrimaryHoveredObject = HoveredObjects.GetComponent<ReactToPointer>();
                time = Time.time;
            }
            else if (Time.time - time > PrimaryHoveredObject.HoverDelay)
                OnPointerTooltipUpdated(PrimaryHoveredObject.TooltipMessage);
        }
        else
        {
            OnPointerTooltipUpdated("");
            PrimaryHoveredObject = null;
            time = 0;
        }
    }
}
