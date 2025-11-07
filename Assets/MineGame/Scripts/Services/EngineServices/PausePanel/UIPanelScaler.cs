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

    public async void CloseAnim()
    {
        inAnim = true;
        Transform panel = GetComponent<RectTransform>().GetChild(0);
        await panel.DOScale(Vector3.one * 0f, 0.5f)
            .SetEase(Ease.InBack, 0.7f)
            .AsyncWaitForCompletion();
        await UniTask.Delay(75);
        inAnim = false;
        gameObject.SetActive(false);
    }
    
    public void OnDestroy()
    {
        DOTween.Kill(gameObject);
    }
}
