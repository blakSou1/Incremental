using Febucci.UI;
using System.Collections;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TextAnimatorPlayer))]
[RequireComponent(typeof(TMP_Text))]
public class TextThrower : MonoBehaviour
{
    private TextAnimatorPlayer _textAnimator;
    private Coroutine _typingSoundCoroutine;

    private void Awake()
    {
        _textAnimator = GetComponent<TextAnimatorPlayer>();
    }

    public void ThrowText(LocString text, VoiceSO voice)
    {
        // Если корутина уже запущена, останавливаем ее
        if (_typingSoundCoroutine != null)
        {
            StopCoroutine(_typingSoundCoroutine);
            _typingSoundCoroutine = null;
        }

        TMP_Text tMP_Text = GetComponent<TMP_Text>();

        tMP_Text.font = voice.font;
        tMP_Text.color = voice.color;
        tMP_Text.material = voice.textMaterial;

        _textAnimator.ShowText("");
        _textAnimator.ShowText(text.ToString());

        // Запускаем корутину для воспроизведения звука
        _typingSoundCoroutine = StartCoroutine(PlayTypingSoundRepeatedly(voice.deltaSound, voice.voice));
    }

    private IEnumerator PlayTypingSoundRepeatedly(int intervalMs, AudioClip sample)
    {
        while (!_textAnimator.textAnimator.allLettersShown)
        {
            sample.PlayAsSoundRandomPitch(0.15f);

            // Ждем указанный интервал
            yield return new WaitForSeconds((float)intervalMs / 1000);
        }
    }
}
