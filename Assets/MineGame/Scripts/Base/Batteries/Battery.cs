using DG.Tweening;
using UnityEngine;
using System.Linq;
using Unity.Mathematics;

public class Battery : MovableObject
{
    public float startEnergy;
    [HideInInspector] public float maxEnergy = 100;
    [HideInInspector] public float currentEnergy;
    public float pointsPerWork = 0.1f;
    
    public bool CheckCharge()
    {
        if (currentEnergy == 0 && !G.GameState.AutoRemoveBroken)
        {
            GameObject g = Instantiate(CMS.GetAll<CMSEntity>().FirstOrDefault(x => x.Is<ConfigItemsInPipe>())!.Get<ConfigItemsInPipe>().boomVFX, transform.position, quaternion.identity);
            g.transform.localScale = Vector3.one * 2;
            R.Audio.boom.PlayAsSoundRandomPitch(0.1f);
        }
        return currentEnergy - 1 >= 0;
    }

    public void Start()
    {
        maxEnergy = startEnergy * (1 + 0.2f * G.GameState.EffLevels);
        currentEnergy = maxEnergy;
    }

    public void ConsumeEnergy()
    {
        currentEnergy -= 1;
        visual.color = Color.Lerp(Color.grey, Color.white, currentEnergy / maxEnergy);
    }

    private void OnCollisionEnter2D(Collision2D other)
    {
        R.Audio.BatteryColisionSound.PlayAsSoundRandomPitch(0.4f);
    }
    
    private Sequence animationSequence;
    
    public void StartAnimation()
    {
        // Останавливаем текущую анимацию, если она есть
        if (animationSequence != null && animationSequence.IsActive())
            animationSequence.Kill();

        // Создаем новую последовательность анимаций
        animationSequence = DOTween.Sequence();
        
        // Сжимаем по X (1.5, 0.7)
        animationSequence.Append(visual.transform.DOScale(new Vector3(1.4f, 0.75f, 1), 0.3f));
        
        // Затем сжимаем по Y (0.7, 1.5)
        animationSequence.Append(visual.transform.DOScale(new Vector3(0.8f, 1.2f, 1), 0.3f));
        animationSequence.Append(visual.transform.DOScale(Vector3.one, 0.2f));
        // Устанавливаем параметры анимации
        animationSequence.SetEase(Ease.InOutQuad);
        animationSequence.SetLoops(-1);
    }

    public void StopAnimation()
    {
        // Останавливаем анимацию и плавно возвращаем к исходному масштабу
        if (animationSequence != null && animationSequence.IsActive())
            animationSequence.Kill(true);

        visual.transform.localScale = Vector3.one;
    }

    private void OnDestroy()
    {
        // Убираем твины при уничтожении объекта
        animationSequence?.Kill();
    }
}
