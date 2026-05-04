using Unity.Services.Analytics;
using Unity.Services.Core;
using UnityEngine;

public class AnaliticksManager : MonoBehaviour, IService
{
    bool isInit = false;

    public void Init()
    {
    }

    public async void Inits()
    {
        try
        {
            await UnityServices.InitializeAsync();
            AnalyticsService.Instance.StartDataCollection();
            isInit = true;
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Analytics init failed: {e.Message}");
        }
    }

    public void EnterGame()
    {
        if (!isInit)
            return;

        CustomEvent myEvent = new("EnterGame")
        {
            {"Enter", true }
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
    }

    public void StartLvl()
    {
        CustomEvent myEvent = new("nextLvl")
        {
            {"Lvlindex", G.run.indexLvl }
        };
        AnalyticsService.Instance.RecordEvent(myEvent);
        AnalyticsService.Instance.Flush();
    }
}
