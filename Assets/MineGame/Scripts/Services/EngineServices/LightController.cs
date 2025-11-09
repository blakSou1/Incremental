using DG.Tweening;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightController : MonoBehaviour, IService
{
    public Light2D globalLight;
    public ConfigLight config;

    public void Init()
    {
        config = CMS.GetAll<CMSEntity>().FirstOrDefault(x => x.Is<ConfigLight>())!.Get<ConfigLight>();
        SetupLight();
    }

    public void SetupLight()
    {
        globalLight = FindObjectsByType<Light2D>(FindObjectsSortMode.None)
            .FirstOrDefault(l => l.lightType == Light2D.LightType.Global);
    }

    public void SetLight(float intensity)
    {
        StartCoroutine(SetLightCoroutine(intensity));
    }

    private IEnumerator SetLightCoroutine(float intensity)
    {
        yield return DOTween.To(
            () => globalLight.intensity,
            x => globalLight.intensity = x,
            intensity,
            config.timeToChangeIntensity
        ).WaitForCompletion();
    }

    public void RestoreLight()
    {
        StartCoroutine(RestoreLightCoroutine());
    }

    private IEnumerator RestoreLightCoroutine()
    {
        yield return DOTween.To(
            () => globalLight.intensity,
            x => globalLight.intensity = x,
            config.timeToChangeIntensity,
            0.25f
        ).WaitForCompletion();
    }

    public void SetColor(Color color)
    {
        StartCoroutine(SetColorCoroutine(color));
    }

    private IEnumerator SetColorCoroutine(Color color)
    {
        yield return DOTween.To(
            () => globalLight.color,
            x => globalLight.color = x,
            color,
            0.75f
        ).WaitForCompletion();
    }
}
