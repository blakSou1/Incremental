using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using System.Collections;

public class Lever : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Slider slider;
    [SerializeField] private float returnSpeed = 0.5f;
    [SerializeField] private float activationThreshold = 0.95f;

    [Header("Events")]
    public UnityEvent onLeverActivated;

    private bool triggerCancelReturning = false;
    private RectTransform sliderRect;
    private Camera mainCamera;

    private void Start()
    {
        sliderRect = slider.GetComponent<RectTransform>();
        mainCamera = Camera.main;
        StartCoroutine(HandleAutoReturnCoroutine());
    }

    public void OnBeginDrag()
    {
        triggerCancelReturning = true;
    }

    public void OnDrag()
    {
        // Конвертируем позицию мыши в локальные координаты слайдера
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            sliderRect,
            Mouse.current.position.ReadValue(),
            mainCamera,
            out Vector2 localPoint
        );

        // Нормализуем положение (0-1) относительно ширины слайдера
        float normalizedValue = Mathf.InverseLerp(
            sliderRect.rect.xMin,
            sliderRect.rect.xMax,
            localPoint.x
        );

        slider.value = Mathf.Clamp01(normalizedValue);
    }

    public void OnEndDrag()
    {
        CheckOutResult();
        StartCoroutine(HandleAutoReturnCoroutine());
    }

    private void CheckOutResult()
    {
        if (slider.value >= activationThreshold && !G.Main.isLockDown)
        {
            onLeverActivated?.Invoke();
        }
        else
        {
            R.Audio.Wrong_Error.PlayAsSoundRandomPitch(0.2f);
        }
    }

    private IEnumerator HandleAutoReturnCoroutine()
    {
        while (!triggerCancelReturning)
        {
            slider.value = Mathf.MoveTowards(slider.value, 0, returnSpeed * Time.deltaTime);
            yield return null; // Ждем следующий кадр
        }

        triggerCancelReturning = false;
    }
}
