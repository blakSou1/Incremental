using DG.Tweening;
using System.Collections;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class UIPanelScaler : MonoBehaviour
{
    public bool inAnim = false;

    public void Close()
    {
        G.SceneLoader.StartCoroutine(CloseAnim());
    }

    public void Open()
    {
        G.SceneLoader.StartCoroutine(CloseAnim(true));
    }

    private IEnumerator CloseAnim(bool isOpen = false)
    {
        if (isOpen)
            gameObject.SetActive(true);

        inAnim = true;
        Transform panel = GetComponent<RectTransform>().GetChild(0);

        yield return panel.DOScale(Vector3.one * (isOpen ? 1f : 0f), 0.5f)
            .SetEase(Ease.InBack, 0.7f)
            .WaitForCompletion();

        yield return new WaitForSeconds(0.075f);
        inAnim = false;

        if (!isOpen)
            gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        DOTween.Kill(gameObject);
    }
}
