using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

[RequireComponent(typeof(RectTransform))]
public class GeneralButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public UnityEvent OnEnter;
    public UnityEvent OnExit;
    public UnityEvent OnClick;

    public void OnPointerEnter(PointerEventData eventData)
    {
        //R.Audio.MouseInButton.PlayAsSoundRandomPitch(0.17f);
        Sequence mySequence = DOTween.Sequence();
        mySequence
            .Append(GetComponent<RectTransform>().GetChild(0).DOScale(Vector3.one * 0.7f, 0.1f).SetEase(Ease.OutBack))
            .Append(GetComponent<RectTransform>().GetChild(0).DOScale(Vector3.one * 0.8f, 0.2f).SetEase(Ease.OutBack));

        OnEnter.Invoke();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<RectTransform>().GetChild(0).DOScale(Vector3.one, 0.3f);
        OnExit.Invoke();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        //R.Audio.part.PlayAsSoundRandomPitch(0.15f);
        Sequence mySequence = DOTween.Sequence();
        mySequence.Append(GetComponent<RectTransform>().GetChild(0).DOScale(Vector3.one * 0.55f, 0.1f))
            .Append(GetComponent<RectTransform>().GetChild(0).DOScale(Vector3.one * 0.8f, 0.1f));
        OnClick.Invoke();
    }

    public void OnDestroy()
    {
        DOTween.Kill(gameObject);
    }
}
