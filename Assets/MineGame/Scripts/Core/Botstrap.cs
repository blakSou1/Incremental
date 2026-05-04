using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

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

        G.gameLogic = new();

        G.AudioManager = CreateSimpleService<AudioManager>();
        G.SceneLoader = CreateSimpleService<SceneLoader>();
        G.LocSystem = CreateSimpleService<LocSystem>();
        G.pausePanel = CreateSimpleService<PausePanel>();
        G.faqPanel = CreateSimpleService<FAQ>();
        G.analiticksManager = CreateSimpleService<AnaliticksManager>();

        G.winAndLouse = new();
        G.run = new();
        G.configGridFunction = Resources.Load<ConfigGridFunction>("IconfigGridFunction"); ;

        G.configGame = CMS.GetAll<CMSEntity>().FirstOrDefault(x => x.Is<ConfigGame>())!.Get<ConfigGame>();

        //
        G.mainEnterPoint = Object.FindFirstObjectByType<MainEnterPoint>();
        G.enemySprite = Object.FindFirstObjectByType<EnemySprite>();
        G.ai = Object.FindFirstObjectByType<AI>();
        G.PlayerController = Object.FindFirstObjectByType<PlayerController>();
        G.volume = Object.FindFirstObjectByType<Volume>();

#if UNITY_EDITOR
        CreateSimpleService<ProgrammerInputTestScript>();
#endif

        G.SceneLoader.onLoadAction = (scene, sceneMode) =>
        {
            G.mainEnterPoint = Object.FindFirstObjectByType<MainEnterPoint>();
            G.enemySprite = Object.FindFirstObjectByType<EnemySprite>();
            G.ai = Object.FindFirstObjectByType<AI>();
            G.PlayerController = Object.FindFirstObjectByType<PlayerController>();
            G.volume = Object.FindFirstObjectByType<Volume>();
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
