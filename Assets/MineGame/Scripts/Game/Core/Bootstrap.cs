using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

public class Bootstrap : MonoBehaviour
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
        G.pausePanel = CreateSimpleService<PausePanel>();
        G.faqPanel = CreateSimpleService<FAQ>();

        G.winAndLouse = new();
        G.run = new();
        G.boardVisualConfig = Resources.Load<BoardVisualConfig>("BoardVisualConfig"); ;

        G.configGame = CMS.GetAll<CMSEntity>().FirstOrDefault(x => x.Is<ConfigGame>())!.Get<ConfigGame>();

        RefreshSceneReferences();

#if UNITY_EDITOR
        CreateSimpleService<ProgrammerInputTestScript>();
#endif

        G.SceneLoader.onLoadAction = (scene, sceneMode) =>
        {
            RefreshSceneReferences();
        };
    }

    private static void RefreshSceneReferences()
    {
        G.mainEnterPoint = Object.FindFirstObjectByType<MainEnterPoint>();
        G.enemySprite = Object.FindFirstObjectByType<EnemySprite>();
        G.ai = Object.FindFirstObjectByType<AI>();
        G.PlayerController = Object.FindFirstObjectByType<PlayerController>();
        G.volume = Object.FindFirstObjectByType<Volume>();
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
