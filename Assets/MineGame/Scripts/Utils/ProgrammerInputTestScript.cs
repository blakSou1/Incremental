using UnityEngine;

public class ProgrammerInputTestScript : MonoBehaviour, IService
{
    private float originalTimeScale = 1f;
    private float originalFixedDeltaTime;
    private const float slowMotionFactor = 0.25f;
    private bool isSlowed = false;
    private bool isPaused = false;
    private float savedTimeScaleBeforePause = 1f;

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

        Debug.Log("Controls initialized:");
        Debug.Log("1 - Debug method");
        Debug.Log("Hold 2 - Slow motion (x0.25)");
        Debug.Log("Press 3 - Pause/Resume");
        Debug.Log("Press 4 - RestartScene");
    }

    private void isDebug()
    {
        if (G.ai != null)
            G.ai.DebugMethod();
    }

    // ЗАМЕДЛЕНИЕ ИГРЫ (Slow motion)
    private void StartSlowMotion()
    {
        if (!isPaused && !isSlowed)
        {
            Time.timeScale = slowMotionFactor;

            Time.fixedDeltaTime = originalFixedDeltaTime * slowMotionFactor;

            isSlowed = true;
            Debug.Log($"Slow motion activated: TimeScale = {Time.timeScale}");
        }
    }

    private void StopSlowMotion()
    {
        if (isSlowed && !isPaused)
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
        isPaused = !isPaused;

        if (isPaused)
        {
            G.inputs.Player.Disable();

            savedTimeScaleBeforePause = Time.timeScale;

            Time.timeScale = 0f;

            Time.fixedDeltaTime = 0f;

            AudioListener.pause = true;

            Debug.Log("Game PAUSED (TimeScale = 0)");
        }
        else
        {
            G.inputs.Player.Enable();

            Time.timeScale = savedTimeScaleBeforePause;

            if (isSlowed)
                Time.fixedDeltaTime = originalFixedDeltaTime * slowMotionFactor;
            else
                Time.fixedDeltaTime = originalFixedDeltaTime;

            AudioListener.pause = false;

            Debug.Log($"Game RESUMED (TimeScale = {Time.timeScale})");
        }
    }

    // Перезагружаем сцену
    public static void RestartCurrentScene()
    {
        var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

        G.SceneLoader.Load(currentScene.name);
    }

    public void RestoreToNormal()
    {
        isPaused = false;
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