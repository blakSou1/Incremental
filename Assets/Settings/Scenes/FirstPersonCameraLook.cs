using UnityEngine;
using UnityEngine.InputSystem;

public class FirstPersonCameraLook : MonoBehaviour
{
    [Header("Чувствительность")]
    [SerializeField] private float mouseSensitivity = 0.15f;

    [Header("Круглый конус обзора")]
    [Tooltip("Базовый радиус обзора в градусах. До этой границы движение свободное.")]
    [SerializeField] private float maxLookRadius = 30f;

    [Tooltip("На сколько градусов можно выйти за границу при усилии.")]
    [SerializeField] private float overshootMax = 10f;

    [Tooltip("Скорость возврата к границе, когда мышь отпущена.")]
    [SerializeField] private float returnSpeed = 60f;

    private float pitch = 0f;
    private float yaw = 0f;
    private Quaternion initialRotation;

    private void Start()
    {
        initialRotation = transform.rotation;
    }

    private void OnEnable()
    {
        G.inputs.Look.Look.performed += OnLook;
        G.inputs.Look.Look.Enable();
    }

    private void OnDisable()
    {
        G.inputs.Look.Look.performed -= OnLook;
        G.inputs.Look.Look.Disable();
    }

    private void OnLook(InputAction.CallbackContext context)
    {
        Vector2 delta = context.ReadValue<Vector2>() * mouseSensitivity;

        Vector2 current = new Vector2(yaw, pitch);
        float radius = current.magnitude;

        float resistance = 1f;
        if (radius > maxLookRadius)
        {
            float t = Mathf.Clamp01((radius - maxLookRadius) / overshootMax); // 0..1
            resistance = 1f - t * t;
        }

        yaw += delta.x * resistance;
        pitch -= delta.y * resistance;

        current = new Vector2(yaw, pitch);
        float r = current.magnitude;
        float hardMax = maxLookRadius + overshootMax;
        if (r > hardMax)
        {
            current = current.normalized * hardMax;
            yaw = current.x;
            pitch = current.y;
        }
    }

    private void Update()
    {
        if (!G.inputs.Look.enabled)
            return;

        Vector2 current = new Vector2(yaw, pitch);
        float radius = current.magnitude;

        if (radius > maxLookRadius)
        {
            float targetRadius = maxLookRadius;
            float newRadius = Mathf.MoveTowards(radius, targetRadius, returnSpeed * Time.deltaTime);
            current = current.normalized * newRadius;
            yaw = current.x;
            pitch = current.y;
        }

        transform.rotation = initialRotation * Quaternion.Euler(pitch, yaw, 0f);
    }

    public void ResetLook()
    {
        yaw = 0f;
        pitch = 0f;
        transform.rotation = initialRotation;
    }
}