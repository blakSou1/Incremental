using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class AnimationControllerUI : AnimationController
{
    [NonSerialized] public new Image _targetRenderer;

    public override void Init()
    {
        _targetRenderer = GetComponent<Image>();
    }

    protected override IEnumerator Anim()
    {
        frame = 0;

        while (frame != _currentAnimation.frames.Count)
        {
            if (_currentAnimation.frames[frame] != null)
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

    public override void SetFlip(bool flipX = false, bool flipY = false) 
    {
        Vector3 scale = _targetRenderer.rectTransform.localScale;
        scale.x = flipX ? -Mathf.Abs(scale.x) : Mathf.Abs(scale.x);
        scale.y = flipY ? -Mathf.Abs(scale.y) : Mathf.Abs(scale.y);
        _targetRenderer.rectTransform.localScale = scale;
    }

}
