using System.Linq;
using UnityEngine;

public class GameState : MonoBehaviour, IService
{
    private ConfigGameStates configGameStates;
    public float Points = 0;
    public int EffLevels = 0;
    public readonly int EffLevelsMax = 6;
    public int RareLevels = 0;
    public readonly int RareLevelsMax = 10;

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
        EffLevels = 0;
        RareLevels = 0;
    }
}
