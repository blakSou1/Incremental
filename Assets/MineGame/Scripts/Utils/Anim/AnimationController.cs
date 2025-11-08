using System.Collections;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    private SpriteRenderer _targetRenderer;
    private AnimationDataSO _currentAnimation;
    private Coroutine _animationCoroutine;

    private int frame = 0;

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
            _animationCoroutine = StartCoroutine(Anim(_currentAnimation));
        }
    }

    public void SetFlip(bool flip) => _targetRenderer.flipX = flip;

    private IEnumerator Anim(AnimationDataSO myAnimData)
    {
        frame = 0;
        while (_currentAnimation == myAnimData)
        {
            _targetRenderer.sprite = myAnimData.frames[frame];
            frame = (frame + 1) % _currentAnimation.frames.Count;

            // Ждем перед сменой кадра
            yield return new WaitForSeconds(1f / myAnimData.framerate);
        }
    }

    private void StopAnimation()
    {
        // Останавливаем текущую корутину, если она существует
        if (_animationCoroutine != null)
        {
            StopCoroutine(_animationCoroutine);
            _animationCoroutine = null;
        }
    }

    private void OnDestroy()
    {
        // Останавливаем анимацию при уничтожении объекта
        StopAnimation();
    }
}
