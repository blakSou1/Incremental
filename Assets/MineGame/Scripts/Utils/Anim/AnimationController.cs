using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class AnimationController : MonoBehaviour
{
    [NonSerialized] public SpriteRenderer _targetRenderer;
    private AnimationDataSO _currentAnimation;
    private Coroutine _animationCoroutine;

    private int frame = 0;

    [HideInInspector] public UnityEvent endAnimation;

    public void Init()
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

    public void SetFlip(bool flip) => _targetRenderer.flipX = flip;

    private IEnumerator Anim()
    {
        frame = 0;

        while (frame != _currentAnimation.frames.Count)
        {
            if(_currentAnimation.frames[frame] != null)
                _targetRenderer.sprite = _currentAnimation.frames[frame];

            Frame frameS = ContainsFrame(frame);
            if (frameS != null)
            {
                frameS.Event?.Invoke();
                yield return new WaitForSeconds((frameS.pause != 0) ? frameS.pause : 1f / _currentAnimation.framerate);
            }
            else
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

    private void StopAnimation()
    {
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;

            endAnimation?.Invoke();
            endAnimation.RemoveAllListeners();
        }
    }

    public IEnumerator FadeCoroutine(bool fadeIn, float fadeDuration, SpriteRenderer sprite)
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

    private void OnDestroy()
    {
        StopAnimation();
    }
}
