using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class AnimationController : MonoBehaviour
{
    [NonSerialized] public SpriteRenderer _targetRenderer;
    protected AnimationDataSO _currentAnimation;
    protected Coroutine _animationCoroutine;

    protected int frame = 0;

    [HideInInspector] public UnityEvent endAnimation;

    public virtual void Init()
    {
        _targetRenderer = GetComponent<SpriteRenderer>();
    }

    public void SetAnimation(AnimationDataSO newAnimation)
    {
        if (_currentAnimation != newAnimation)
        {
            StopAnimation();

            _currentAnimation = newAnimation;
            _animationCoroutine = StartCoroutine(Anim());
        }
    }

    public virtual void SetFlip(bool flipX = false, bool flipY = false)
    {
        _targetRenderer.flipX = flipX;
        _targetRenderer.flipY = flipY;
    }

    protected virtual IEnumerator Anim()
    {
        frame = 0;

        while (frame != _currentAnimation.frames.Count)
        {
            if(_currentAnimation.frames[frame] != null)
                _targetRenderer.sprite = _currentAnimation.frames[frame];

            Frame frameS = ContainsFrame(frame);
            frameS?.Event?.Invoke();

            yield return new WaitForSeconds(1f / _currentAnimation.framerate);

            frame++;

            if (_currentAnimation.isLoop)
                frame = frame % _currentAnimation.frames.Count;
        }

        StopAnimation();
    }

    public Frame ContainsFrame(int indexFrame)
    {
        return _currentAnimation.framesSettings.FirstOrDefault(frame => frame.indexFrame == indexFrame);
    }

    protected void StopAnimation()
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;

            endAnimation?.Invoke();
            endAnimation.RemoveAllListeners();
        }
    }

    Coroutine _animationCoroutineFabe;
    public void SetFadeCoroutine(bool fadeIn, float fadeDuration, SpriteRenderer sprite)
    {
        if (_animationCoroutineFabe != null)
            StopCoroutine(_animationCoroutineFabe);

        _animationCoroutineFabe = StartCoroutine(FadeCoroutine(fadeIn, fadeDuration, sprite));
    }

    private IEnumerator FadeCoroutine(bool fadeIn, float fadeDuration, SpriteRenderer sprite)
    {
        if (sprite == null) yield break;

        float startAlpha = sprite.color.a;
        float targetAlpha = fadeIn ? 1f : 0f;

        float elapsedTime = 0f;

        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);

            float currentAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);

            Color color = sprite.color;
            color.a = currentAlpha;
            sprite.color = color;

            yield return null;
        }

        Color finalColor = sprite.color;
        finalColor.a = targetAlpha;
        sprite.color = finalColor;
    }

    protected void OnDestroy()
    {
        StopAnimation();
    }
}
