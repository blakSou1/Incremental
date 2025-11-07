using UnityEngine;
using System.Linq;

public class GameState : MonoBehaviour, IService
{
    private ConfigGameStates configGameStates;
    public float Points = 0;
    public int EffLevels = 0;
    public readonly int EffLevelsMax = 6;
    public int RareLevels = 0;
    public readonly int RareLevelsMax = 10;
    public int SlotsLevels = 0;
    public readonly int SlotLevelsMax = 7;
    public int AutoSpawnLevels = 0;
    public readonly int AutoSpawnLevelsMax = 3;
    public bool AutoRemoveBroken = false;
    
    public void Init()
    {
        configGameStates =
            CMS.GetAll<CMSEntity>().FirstOrDefault(x => x.Is<ConfigGameStates>())!.Get<ConfigGameStates>();
        if (configGameStates.overrideValues)
        {
            Points = configGameStates.overridePoints;
        }
    }

    public void Reset()
    {
        Points = 0;
        SlotsLevels = 0;
        EffLevels = 0;
        RareLevels = 0;
    }
}
