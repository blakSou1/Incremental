using System.Collections;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Events;
using UnityEngine.Playables;

[RequireComponent(typeof(Animator))]
public class ClipPlayer : MonoBehaviour
{
    private PlayableGraph graph;
    private AnimationMixerPlayable mixer;
    private AnimationClipPlayable current;
    private Coroutine endRoutine;

    public readonly UnityEvent<AnimationClip> onClipEnd = new();

    private void Awake()
    {
        var animator = GetComponent<Animator>();

        graph = PlayableGraph.Create("ClipPlayer");
        var output = AnimationPlayableOutput.Create(graph, "Anim", animator);

        mixer = AnimationMixerPlayable.Create(graph, 1);
        output.SetSourcePlayable(mixer);

        graph.Play();
    }

    /// <summary>Проигрывает любой AnimationClip, даже если его нет в контроллере.</summary>
    public void Play(AnimationClip clip, float speed = 1f, float startTime = 0f)
    {
        if (clip == null) return;

        if (current.IsValid())
        {
            mixer.DisconnectInput(0);
            current.Destroy();
        }

        current = AnimationClipPlayable.Create(graph, clip);
        current.SetSpeed(speed);
        current.SetTime(startTime);

        mixer.ConnectInput(0, current, 0);
        mixer.SetInputWeight(0, 1f);

        if (!graph.IsPlaying())
            graph.Play();

        // Перезапускаем корутину ожидания
        if (endRoutine != null)
            StopCoroutine(endRoutine);

        endRoutine = StartCoroutine(WaitEnd(clip, speed));
    }

    public void Stop()
    {
        if (current.IsValid())
        {
            mixer.DisconnectInput(0);
            current.Destroy();
        }
    }

    public bool IsPlaying =>
        current.IsValid() && current.GetTime() < current.GetDuration();

    private IEnumerator WaitEnd(AnimationClip clip, float speed)
    {
        float duration = clip.length / Mathf.Max(speed, 0.0001f);
        yield return new WaitForSeconds(duration);

        onClipEnd.Invoke(clip);
        endRoutine = null;
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

    private void OnDestroy()
    {
        if (graph.IsValid())
            graph.Destroy();
    }
}