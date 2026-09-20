using System.Linq;
using UnityEngine;

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

        WinAndLoss winAndLouse = new();
        HpManager managerHp = new();
        G.run = new();
        G.eventManager = new();
        winAndLouse.Init();
        managerHp.Init();
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
        G.enemySprite = Object.FindFirstObjectByType<EnemySprite>();
        G.ai = Object.FindFirstObjectByType<AI>();
        G.PlayerController = Object.FindFirstObjectByType<PlayerController>();
        G.mainEnterPoint = Object.FindFirstObjectByType<MainEnterPoint>();

        GameObject.Destroy(G.eventManager.host);
        G.eventManager.host = CreateSimpleService<CoroutineHost>();

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

public class CoroutineHost : MonoBehaviour, IService
{
    public void Init()
    {
    }
}