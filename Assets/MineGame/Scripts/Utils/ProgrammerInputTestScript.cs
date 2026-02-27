using UnityEditor;
using UnityEngine;

public class ProgrammerInputTestScript : MonoBehaviour, IService
{
    private float originalTimeScale = 1f;
    private float originalFixedDeltaTime;
    private const float slowMotionFactor = 0.25f;
    private bool isSlowed = false;

    public void Init()
    {
        G.inputs.Debug._1.started += i => isDebug();

        originalTimeScale = Time.timeScale;
        originalFixedDeltaTime = Time.fixedDeltaTime;

        // Замедление игры (slow motion)
        G.inputs.Debug._2.started += i => StartSlowMotion();
        G.inputs.Debug._2.canceled += i => StopSlowMotion();

        // Пауза
        G.inputs.Debug._3.started += i => TogglePause();

        G.inputs.Debug._4.started += i => RestartCurrentScene();

        G.inputs.Debug._5.started += i => Win();
        G.inputs.Debug._6.started += i => Louse();

        Debug.Log("Controls initialized:");
        Debug.Log("1 - Debug method");
        Debug.Log("Hold 2 - Slow motion (x0.25)");
        Debug.Log("Press 3 - Pause/Resume");
        Debug.Log("Press 4 - RestartScene");
        Debug.Log("Press 5 - Win");
        Debug.Log("Press 6 - Louse");
    }

    private void isDebug()
    {
        if (G.ai != null)
            G.ai.DebugMethod();
    }

    // ЗАМЕДЛЕНИЕ ИГРЫ (Slow motion)
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

    // ПАУЗА (полная остановка)
    private void TogglePause()
    {
#if UNITY_EDITOR
        EditorApplication.isPaused = true;
#endif
        Debug.Log("Game PAUSED");
    }

    // Перезагружаем сцену
    public static void RestartCurrentScene()
    {
        var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

        G.SceneLoader.Load(currentScene.name);
    }

    private void Win()
    {
        if(G.gameMode != null)
            G.winAndLouse.WinPlayer();
    }
    private void Louse()
    {
        if (G.gameMode != null)
            G.winAndLouse.WinEnemy();
    }

    public void RestoreToNormal()
    {
        isSlowed = false;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = originalFixedDeltaTime;
        AudioListener.pause = false;
        Debug.Log("Game state fully restored to normal");
    }

    private void OnDestroy()
    {
        RestoreToNormal();

        if (G.inputs != null)
        {
            G.inputs.Debug._2.started -= i => StartSlowMotion();
            G.inputs.Debug._2.canceled -= i => StopSlowMotion();
            G.inputs.Debug._3.started -= i => TogglePause();
        }
    }
}