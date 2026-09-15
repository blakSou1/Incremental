using UnityEditor;
using UnityEngine;

public class ProgrammerInputTestScript : MonoBehaviour, IService
{
    private float originalTimeScale = 1f;
    private float originalFixedDeltaTime;
    private const float slowMotionFactor = 0.25f;
    private bool isSlowed = false;

    private Inputs inp;

    public void Init()
    {
        inp = new();
        inp.Enable();

        originalTimeScale = Time.timeScale;
        originalFixedDeltaTime = Time.fixedDeltaTime;

        inp.Debug._1.started += i => isDebug();
        inp.Debug._2.started += i => StartSlowMotion();
        inp.Debug._2.canceled += i => StopSlowMotion();
        inp.Debug._3.started += i => TogglePause();
        inp.Debug._4.started += i => RestartCurrentScene();
        inp.Debug._5.started += i => Win();
        inp.Debug._6.started += i => Louse();
        inp.Debug._7.started += i => Damage();
        inp.Debug._8.started += i => Scen();

        Debug.Log("Controls initialized:");
        Debug.Log("1 - Debug method");
        Debug.Log("Hold 2 - Slow motion (x0.25)");
        Debug.Log("Press 3 - Pause/Resume");
        Debug.Log("Press 4 - RestartScene");
        Debug.Log("Press 5 - Win");
        Debug.Log("Press 6 - Louse");
        Debug.Log("Press 7 - Damage Player");
    }

    private void isDebug()
    {
        if (G.ai != null)
            G.ai.DebugMethod();
    }
    private void Scen()
    {
        if (G.DamageEnemyScenario != null)
            G.DamageEnemyScenario.StartCoroutine(G.DamageEnemyScenario.StartScenario());
    }

    private void Damage()
    {
        StartCoroutine(G.enemyHp.DamagePlayer(1));
    }

    private void StartSlowMotion()
    {
        if (!isSlowed)
        {
            Time.timeScale = slowMotionFactor;

            Time.fixedDeltaTime = originalFixedDeltaTime * slowMotionFactor;

            isSlowed = true;
            Debug.Log($"Slow motion activated: TimeScale = {Time.timeScale}");
        }
    }

    private void StopSlowMotion()
    {
        if (isSlowed)
        {
            Time.timeScale = originalTimeScale;
            Time.fixedDeltaTime = originalFixedDeltaTime;

            isSlowed = false;
            Debug.Log($"Normal speed restored: TimeScale = {Time.timeScale}");
        }
    }

    private void TogglePause()
    {
#if UNITY_EDITOR
        EditorApplication.isPaused = true;
#endif
    }

    public static void RestartCurrentScene()
    {
        var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

        G.SceneLoader.Load(currentScene.name);
    }

    private void Win()
    {
        if(G.mainEnterPoint != null)
            G.eventManager.PlayerWin.Invoke();
    }
    private void Louse()
    {
        if (G.mainEnterPoint != null)
            G.eventManager.PlayerLose.Invoke();
    }

    public void RestoreToNormal()
    {
        isSlowed = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = originalFixedDeltaTime;
        AudioListener.pause = false;
    }

    private void OnDestroy()
    {
        RestoreToNormal();

        if (inp != null)
        {
            inp.Debug._2.started -= i => StartSlowMotion();
            inp.Debug._2.canceled -= i => StopSlowMotion();
            inp.Debug._3.started -= i => TogglePause();
            inp.Debug._4.started -= i => RestartCurrentScene();
            inp.Debug._5.started -= i => Win();
            inp.Debug._6.started -= i => Louse();
            inp.Debug._7.started -= i => Damage();
            inp.Debug._8.started -= i => Scen();
        }
    }
}