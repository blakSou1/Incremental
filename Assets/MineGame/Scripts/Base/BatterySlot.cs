using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class BatterySlot : MonoBehaviour
{
    public MovableObject currentBattery { get; private set; }
    public bool isClosed = true;
    
    private Transform posToMoveBat;
    private Slider bar;
    
    private void Awake()
    {
        posToMoveBat = transform.GetChild(0);
        bar = GetComponentInChildren<Slider>(true);
    }

    public bool TryConsume()
    {
        if (currentBattery == null) return true;

        if (currentBattery.Is(out Battery b))
        {
            if (!b.CheckCharge())
            {
                if (G.GameState.AutoRemoveBroken)
                {
                    RemoveBattery();
                    return true;
                }
                return false;
            }
            b.ConsumeEnergy();
        }
        if (currentBattery.Is(out Resistor r))
        {
            if (!r.CheckCharge())
            {
                if (G.GameState.AutoRemoveBroken)
                {
                    RemoveBattery();
                    return true;
                }
                return false;
            }
            r.ConsumeEnergy();
        }
        
        ChangeBar(currentBattery);
        return true;
    }

    public void SetBattery(MovableObject battery)
    {
        if (currentBattery != null)
        {
            if (currentBattery.Is(out Battery b))
                b.StopAnimation();
            if (currentBattery.Is(out Artefact a))
                a.StopAnimation();

            currentBattery.transform.SetParent(null);
            currentBattery.mySlot = null;
            currentBattery.ChangeRB(true);
            currentBattery.visual.sortingOrder = 10;
        }

        currentBattery = battery;
        battery.transform.SetParent(transform);
        R.Audio.batterySet.PlayAsSoundRandomPitch(0.2f);
        currentBattery.visual.sortingOrder = 3;
        ChangeBar(currentBattery);
        
        if (currentBattery.Is(out Battery b2) && G.Main.Machine.isWorking)
            b2.StartAnimation();
        if (currentBattery.Is(out Artefact a2) && G.Main.Machine.isWorking)
            a2.StartAnimation();
    }

    public void RemoveBattery()
    {
        if (currentBattery != null)
        {
            currentBattery.transform.SetParent(null);
            if (currentBattery.Is(out Battery b))
                b.StopAnimation();

            if (currentBattery.Is(out Artefact a))
                a.StopAnimation();

            currentBattery.mySlot = null;
            currentBattery.ChangeRB(true);
            R.Audio.batteryRunOut.PlayAsSoundRandomPitch(0.2f);
            ChangeBar(null);
            currentBattery.visual.sortingOrder = 10;
            currentBattery = null;
        }
    }

    public void ChangeBar(MovableObject obj)
    {
        if (obj == null)
        {
            bar.gameObject.SetActive(false);
            return;
        }

        if (obj.Is(out Battery b))
        {
            bar.gameObject.SetActive(true);
            bar.value = b.currentEnergy / b.maxEnergy;
        }
        else if (obj.Is(out Resistor r))
        {
            bar.gameObject.SetActive(true);
            bar.value = r.currentEnergy / r.maxEnergy;
        }
        else
        {
            bar.gameObject.SetActive(false);
        }
        
    }

    public bool HaveBattery() => currentBattery != null;
    public Vector3 GetMyPos() => posToMoveBat.position;

    public void OpenSlot()
    {
        OpenCover();
        isClosed = false;
    }
    
    private void OpenCover()
    {
        Transform cover = transform.GetChild(1);
        cover.GetComponent<SpriteRenderer>().sortingOrder = 20;
        R.Audio.OpenCover.PlayAsSoundRandomPitch(0.1f);
        DOTween.Sequence()
            // Основные параметры движения
            .Join(cover.DOJump(
                endValue: cover.transform.position + Vector3.right * 8f + Vector3.down * 11f,  // 5 метров вправо
                jumpPower: 8f,                                         // Высота прыжка 3м
                numJumps: 1,                                           // Один прыжок
                duration: 2f                                         // За 1.7 секунды
            ).SetEase(Ease.OutQuad))
            
            // Параметры вращения
            .Join(cover.DORotate(
                endValue: new Vector3(0, 0, 90f),                     // 2 полных оборота (720°)
                duration: 2,                                        // Синхронизировано с движением
                mode: RotateMode.FastBeyond360
            ).SetEase(Ease.Linear))
            .Join(cover.transform.DOPunchScale
                (Vector3.one * 0.4f, 
                0.3f, 
                elasticity: 0f, 
                vibrato: 0))
            
            // Задержка перед исчезновением (0.3 сек)
            .AppendInterval(0.3f)
            
            // Автоматическое удаление объекта
            .OnComplete(() => Destroy(cover.gameObject));
    }
}
