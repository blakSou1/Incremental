using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using Unity.Mathematics;

public class TVLogic : MonoBehaviour
{
    [SerializeField] private Transform pointToSpawn;
    [SerializeField] private TMP_Text percentText;
    [SerializeField] private Slider bar;
    [SerializeField] private BatterySlot slot;
    private List<int> milestones;
    private float pointsEver = 0;
    private int currentMilestone = 0;
    public void Start()
    {
        milestones = new List<int>() { 3000, 4500, 6500 };
        AddPoints(0);
    }

    public void AddPoints(float points)
    {
        pointsEver += points;
        if (currentMilestone == 3)
        {
            percentText.text = new LocString("Completed!","Выполнено!").ToString();
            bar.value = 1;
        }
        if (currentMilestone != 3 && pointsEver > milestones[currentMilestone])
        {
            pointsEver -= milestones[currentMilestone];
            SpawnCassete(currentMilestone);
            R.Audio.positive.PlayAsSound(-0.3f);
            currentMilestone++;
        }
        if (currentMilestone != 3)
        {
            percentText.text = ((pointsEver * 100/ milestones[currentMilestone]).ToString("F2")) + "%";
            bar.value = (pointsEver / milestones[currentMilestone]);
        }
    }

    public void SpawnCassete(int id)
    {
        Instantiate(CMS.GetAll<CMSEntity>().FirstOrDefault(x => x.Is<CasseteList>())!.Get<CasseteList>()
            .CassetePrefabs[id + 1], pointToSpawn.position, quaternion.identity);
    }
    public void PlayCassete()
    {
        if (slot.currentBattery != null)
        {
            R.Audio.positive.PlayAsSound(0.3f);
            slot.currentBattery.Is(out Cassete cassete);
            _ = G.SceneLoader.LoadAdditive(cassete.multName);
        }
    }
}

[Serializable]
public class CasseteList : EntityComponentDefinition
{
    public List<GameObject> CassetePrefabs;
}
