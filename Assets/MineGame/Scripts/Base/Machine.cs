using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;

public class Machine : MonoBehaviour
{
    [SerializeField] private TMP_Text pointPerSecond;
    public TextThrower labelText;
    public List<BatterySlot> slots;
    private bool stopWorkingTrigger = false;
    public bool isWorking = false;
    private int loopId;

    private void Awake()
    {
        slots = GetComponentsInChildren<BatterySlot>().ToList();
    }

    public async void TryRunMachine()
    {
        if (CheckForOneBattery())
        {
            if (!isWorking)
                MachineWork().Forget();
            else
            {
                await labelText.ThrowText(new LocString("Already working!!", "Уже работает!!"), R.normalVoice);
                await UniTask.Delay(100);
                _ = labelText.ThrowText(new LocString("<wave>Working!", "<wave>Работает!"), R.normalVoice);
            }
        }
        else
        {
            _ = labelText.ThrowText(new LocString("Not enough batteries!", "Нехватает батареек!"), R.normalVoice);
        }
    }

    public bool CheckForOneBattery()
    {
        return slots.FirstOrDefault(x => !x.isClosed && x.HaveBattery()) != null;
    }

    public async UniTask MachineWork()
    {
        isWorking = true;
        await UniTask.Delay(500);
        R.Audio.MachineStarted.PlayAsSound();
        G.Main.MainCamera.GetComponent<CameraShake>().Shake(1.5f);
        loopId = G.AudioManager.PlayLoop(R.Audio.factorybg, 2);
        await UniTask.Delay(300);
        
        _ = labelText.ThrowText(new LocString("<wave>Working!", "<wave>Работает!"), R.normalVoice);
        ChangeBatteriesAnim(true);

        while (!stopWorkingTrigger && CheckForOneBattery())
        {
            await UniTask.Delay(100);

            List<BatterySlot> temp = slots.FindAll(x => !x.isClosed).ToList();
            for (int i = 0; i < temp.Count; i++)
            {
                if (slots[i].currentBattery.Is(out Duck d))
                {
                    StopFunction();
                    _ = DestroyAllItems();
                    return;
                }
                if (!slots[i].TryConsume())
                {
                    StopFunction();
                    return;
                }

                if (G.Main.isLockDown)
                {
                    StopFunction();
                    return;
                }
            } 
            float suma = CalculatePointSum();
            pointPerSecond.text = (suma*10).ToString("F2") + "/s";
            G.GameState.Points += suma;
            G.Main.TV.AddPoints(suma);
        }
        
        if (!CheckForOneBattery())
            StopFunction();
    }
    private async void StopFunction()
    {
        if (G.Main.isLockDown)
        {
            isWorking = false;
            stopWorkingTrigger = false;
            ChangeBatteriesAnim(false);
            G.AudioManager.RemoveLoop(loopId);
            return;
        }
        pointPerSecond.text = "0/s";
        ChangeBatteriesAnim(false);
        R.Audio.MachineStopped.PlayAsSound(0.4f);
        G.Main.MainCamera.GetComponent<CameraShake>().Shake(0.4f);
        isWorking = false;
        stopWorkingTrigger = false;
        G.AudioManager.RemoveLoop(loopId);
        
        if(labelText.gameObject.activeSelf) _ = labelText.ThrowText(new LocString("Need Reload!", "Нужна перезагрузка машины!"), R.normalVoice);
    }

    public async UniTask DestroyAllItems()
    {
        List<BatterySlot> temp = slots.FindAll(x => !x.isClosed).ToList();
        DecrementalDelayTimer timer = new(350, 175, 0.75f);
        R.Audio.DuckBaimit.PlayAsSound(0.3f);

        foreach (var s in temp)
        {
            if (s.currentBattery != null)
            {
                G.Main.MainCamera.GetComponent<CameraShake>().Shake(2);
                MovableObject m = s.currentBattery;
                s.RemoveBattery();
                GameObject g = Instantiate(CMS.GetAll<CMSEntity>().FirstOrDefault(x => x.Is<ConfigItemsInPipe>())!.Get<ConfigItemsInPipe>().boomVFX, m.transform.position, Quaternion.identity);
                g.transform.localScale = Vector3.one * 2;
                R.Audio.boom.PlayAsSoundRandomPitch(0.1f);
                m.body.AddForce(new Vector2(1500, 3000));
                await UniTask.Delay(timer.GetDelay());
            }
        }
    }

    private void ChangeBatteriesAnim(bool toStart)
    {
        List<BatterySlot> t = slots.FindAll(x => !x.isClosed).ToList();
        for (int i = 0; i < t.Count; i++)
        {
            if (slots[i].currentBattery.Is(out Battery b))
            {
                if(toStart) b.StartAnimation();
                else b.StopAnimation();
            }
            if (slots[i].currentBattery.Is(out Artefact a))
            {
                if(toStart) a.StartAnimation();
                else a.StopAnimation();
            }
        }
    }
    public float CalculatePointSum()
    {
        float sum = 0;
        float multi = 1;
        List<BatterySlot> t = slots.FindAll(x => !x.isClosed).ToList();
        for (int i = 0; i < t.Count; i++)
        {
            if (slots[i].currentBattery.Is(out Battery b))
                sum += b.pointsPerWork;
            if (slots[i].currentBattery.Is(out Resistor r))
                multi += r.multiPoints;
            if (slots[i].currentBattery.Is(out Artefact a))
                multi += a.multiPoints;
        }

        return sum * multi;
    }
    public void StopMachine()
    {
        stopWorkingTrigger = true;
    }
}
