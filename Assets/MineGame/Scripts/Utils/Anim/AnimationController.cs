using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class AnimationController : MonoBehaviour
{
    private SpriteRenderer _targetRenderer;
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
            _targetRenderer.sprite = _currentAnimation.frames[frame];
            frame++;

            if (_currentAnimation.isLoop)
                frame = frame % _currentAnimation.frames.Count;

            yield return new WaitForSeconds(1f / _currentAnimation.framerate);
        }
        StopAnimation();
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

    private void OnDestroy()
    {
        StopAnimation();
    }
}
