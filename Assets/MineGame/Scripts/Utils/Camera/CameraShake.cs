using DG.Tweening;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    [Header("Настройки тряски")]
    [SerializeField] private float duration = 1.6f;  
    [SerializeField] private float strength = 0.4f;  
    [SerializeField] private int vibrato = 8;        
    [SerializeField] private float randomness = 90f; 
    [SerializeField] private bool snapping = false;  

    private Vector3 _originalPos;
    private Transform _cameraTransform;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
        _originalPos = _cameraTransform.localPosition;
        G.cameraShake = this;
    }

    public void Shake(float force = 1)
    {
        _cameraTransform.DOKill();
        _cameraTransform.localPosition = _originalPos;

        _cameraTransform.DOShakePosition(
            duration,
            strength * force,
            vibrato,
            randomness,
            snapping,
            fadeOut: true
        ).SetEase(Ease.OutQuad); 
    }
    public void SpecialShake(float force, float time)
    {
        _cameraTransform.DOKill();
        _cameraTransform.localPosition = _originalPos;

        _cameraTransform.DOShakePosition(
            duration * time,
            strength * force,
            vibrato,
            randomness,
            snapping,
            fadeOut: true
        ).SetEase(Ease.OutQuad); 
    }
}