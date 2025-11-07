using System;
using UnityEngine;
using Cysharp.Threading.Tasks;

public class TextWithButtonComp : MonoBehaviour
{
    [SerializeField] public TextThrower _textThrower;
    [SerializeField] public GeneralButton _button;
    
    private bool trigger = false;

    private void Awake()
    {
        _button.OnClick.AddListener(() => _button.gameObject.SetActive(false));
        _button.OnClick.AddListener(() => trigger = true);
    }

    public void SetActive(bool b)
    {
        _textThrower.gameObject.SetActive(b);
    }
    public async UniTask WriteTextWithoutButton(LocString text)
    {
        _textThrower.gameObject.SetActive(true);
        await UniTask.Yield();
        await _textThrower.ThrowText(text, R.normalVoice);
    }

    public async UniTask WriteTextWithButton(LocString text)
    {
        _textThrower.gameObject.SetActive(true);
        await UniTask.Yield();
        await _textThrower.ThrowText(text, R.ubiVoice);
        _button.gameObject.SetActive(true);
        trigger = false;
        await UniTask.WaitUntil(() => trigger);
        _button.gameObject.SetActive(false);
    }
}
