using UnityEngine;

public class Botstrap : MonoBehaviour
{
    void Start()
    {
        GameBootstrapper.Init();

        G.SceneLoader.Load("MainMenu");
    }
}

public static class GameBootstrapper
{
    private static GameObject serviceHolder;

    public static void Init()
    {
        Application.targetFrameRate = 60;

        CMS.Init();

        R.InitAll();

        serviceHolder = new GameObject("===Services===");
        Object.DontDestroyOnLoad(serviceHolder);

        G.inputs = new();
        G.inputs.Enable();

        G.AudioManager = CreateSimpleService<AudioManager>();
        G.SceneLoader = CreateSimpleService<SceneLoader>();
        G.LocSystem = CreateSimpleService<LocSystem>();
        G.LightController = CreateSimpleService<LightController>();
        G.GameState = CreateSimpleService<GameState>();
        G.pausePanel = CreateSimpleService<PausePanel>();

        G.SceneLoader.onLoadAction = (scene, sceneMode) =>
        {                
            G.LightController.SetupLight();

            G.Main = Object.FindFirstObjectByType<GameMain>();
        };
    }
    
    private static T CreateSimpleService<T>() where T : Component, IService
    {
        GameObject g = new(typeof(T).ToString());

        g.transform.parent = serviceHolder.transform;
        T t = g.AddComponent<T>();
        t.Init();
        return g.GetComponent<T>();
    }
}

public interface IService
{
    public void Init();
}
