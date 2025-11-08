using UnityEngine;
using DG.Tweening;
using System.Collections;

public static class MyTweens
{
    public static IEnumerator DOScaleSaveBounce(this Transform source, Vector3 resultPos, float time)
    {
        // Запускаем анимацию масштабирования
        var tween = source.DOScale(resultPos, time).SetEase(Ease.InOutBack);

        // Ожидаем завершения анимации
        while (tween.IsActive() && !tween.IsComplete())
        {
            // Проверяем, не стал ли масштаб отрицательным
            var s = source.localScale;
            if (s.x < 0 || s.y < 0 || s.z < 0)
            {
                source.localScale = Vector3.zero;
            }
            yield return null; // Ждем следующего кадра
        }
    }
}
