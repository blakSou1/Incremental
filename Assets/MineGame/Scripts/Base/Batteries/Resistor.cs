using UnityEngine;
using System.Linq;
using Unity.Mathematics;

public class Resistor : MovableObject
{
    public float multiPoints = 0;
    public float startEnergy;
    [HideInInspector] public float maxEnergy = 100;
    [HideInInspector] public float currentEnergy;
    
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
        visual.material.SetFloat("_Glow", (maxEnergy - currentEnergy) * 10 / maxEnergy);
    }
    private void OnCollisionEnter2D(Collision2D other)
    {
        R.Audio.ResistorSound.PlayAsSoundRandomPitch(0.4f);
    }
}
