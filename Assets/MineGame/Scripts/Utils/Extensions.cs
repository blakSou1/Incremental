using DG.Tweening;
using UnityEngine;

public static class Extensions
{
    public static void ShowUp(GameObject obj, float duration = 0.75f, float elasticity = 1.1f)
    {
        Vector3 targetScale = obj.transform.localScale;
        obj.transform.localScale = Vector3.zero;
        obj.transform.DOScale(targetScale, duration)
            .SetEase(Ease.OutElastic, elasticity, 0.5f);
    }
    public static void HideDown(this GameObject obj, float duration = 0.75f)
    {
        obj.transform.DOScale(Vector3.zero, duration)
            .SetEase(Ease.InBack);
    }
}
