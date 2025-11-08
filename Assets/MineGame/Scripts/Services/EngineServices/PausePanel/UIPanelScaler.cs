using UnityEngine;
using DG.Tweening;
using Cysharp.Threading.Tasks;

[RequireComponent(typeof(RectTransform))]
public class UIPanelScaler : MonoBehaviour
{
    public bool inAnim = false;
    
    public void Close()
    {
        CloseAnim();
    }
    public void Open()
    {
        CloseAnim(true);
    }

    private async void CloseAnim(bool isOpen = false)
    {
        if(isOpen)
            gameObject.SetActive(true);

        inAnim = true;
        Transform panel = GetComponent<RectTransform>().GetChild(0);
        await panel.DOScale(Vector3.one * (isOpen? 1f: 0f), 0.5f)
            .SetEase(Ease.InBack, 0.7f)
            .AsyncWaitForCompletion();
        await UniTask.Delay(75);
        inAnim = false;

        if (!isOpen)
            gameObject.SetActive(false);
    }
    
    public void OnDestroy()
    {
        DOTween.Kill(gameObject);
    }
}
