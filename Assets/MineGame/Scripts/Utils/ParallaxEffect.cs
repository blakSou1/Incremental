using UnityEngine;
using UnityEngine.InputSystem;

public class ParallaxEffect : MonoBehaviour
{
    [Header("Настройки параллакса")]
    [SerializeField, Range(0f, 1f)]
    private float _parallaxStrength = 0.5f; 

    [SerializeField]
    private bool _useScreenCenter = true;

    private RectTransform _rectTransform;
    private Vector2 _initialPosition;
    private Vector2 _screenCenter;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _initialPosition = _rectTransform.anchoredPosition;

        if (_useScreenCenter)
        {
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas != null)
            {
                Rect canvasRect = canvas.GetComponent<RectTransform>().rect;
                _screenCenter = new Vector2(canvasRect.width * 0.5f, canvasRect.height * 0.5f);
            }
            else
                _screenCenter = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
        }
    }

    private void Update()
    {
        Vector2 inputPosition = Mouse.current.position.ReadValue();

        if (_useScreenCenter)
        {
            Vector2 offset = (inputPosition - _screenCenter) * _parallaxStrength;
            _rectTransform.anchoredPosition = _initialPosition - offset;
        }
        else
        {
            Vector2 offset = inputPosition * _parallaxStrength;
            _rectTransform.anchoredPosition = _initialPosition - offset;
        }
    }
}