using UnityEngine;
using DG.Tweening;

public class Cassete : MovableObject
{
    public bool isSpecial = false;
    public string multName;
    private Material _material;
    private Sequence _animationSequence;
    private Color _originalColor;

    private void OnCollisionEnter2D(Collision2D other)
    {
        R.Audio.CasseteSound.PlayAsSoundRandomPitch(0.2f);
    }

    private void Start()
    {
        if (isSpecial)
        {
            _material = visual.material;
            _originalColor = _material.GetColor("_GlowColor");
            StartAnimation();
        }
    }

    public void StartAnimation()
    {
        _animationSequence = DOTween.Sequence();

        DOTween.To(
            () => _material.GetFloat("_Glow"),
            value => _material.SetFloat("_Glow", value),
            30f,
            0.25f
        );
        float t = 0.7f;
        
        // 2. Циклическое изменение цвета
        _animationSequence.Append(
            DOTween.To(
                () => _material.GetColor("_GlowColor"),
                value => _material.SetColor("_GlowColor", value),
                Color.red,
                t/2
            )
        );
        
        _animationSequence.Append(
            DOTween.To(
                () => _material.GetColor("_GlowColor"),
                value => _material.SetColor("_GlowColor", value),
                Color.blue,
                t
            )
        );

        _animationSequence.Append(
            DOTween.To(
                () => _material.GetColor("_GlowColor"),
                value => _material.SetColor("_GlowColor", value),
                Color.magenta,
                t
            )
        );
        
        _animationSequence.Append(
            DOTween.To(
                () => _material.GetColor("_GlowColor"),
                value => _material.SetColor("_GlowColor", value),
                Color.cyan,
                t
            )
        );

        _animationSequence.Append(
            DOTween.To(
                () => _material.GetColor("_GlowColor"),
                value => _material.SetColor("_GlowColor", value),
                Color.red,
                t/2
            )
        );

        // Зацикливаем анимацию цвета
        _animationSequence.SetLoops(-1, LoopType.Restart);
    }

    public void StopAnimation()
    {
        if (_animationSequence != null)
        {
            _animationSequence.Kill();
            
            DOTween.To(() => _material.GetFloat("_Glow"), 
                value => _material.SetFloat("_Glow", value), 
                0, 
                0.25f);
            
            DOTween.To(() => _material.GetColor("_GlowColor"), 
                value => _material.SetColor("_GlowColor", value), 
                _originalColor, 
                0.25f);
        }
    }

    private void OnDestroy()
    {
        _animationSequence?.Kill();
    }
}
