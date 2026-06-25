using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "IHelper", menuName = "IHelper")]
public class IHelper : ScriptableObject
{
    float modifVolume = 0;
    public static UnityEvent customEventAnim = new();

    public void InvokeEvent()
    {
        customEventAnim?.Invoke();
        customEventAnim.RemoveAllListeners();
    }

    public void Pass()
    {
        G.mainEnterPoint.StartCoroutine(G.conditionsOfVictoryAndDefeat.Pass());
    }

    public void SetRu()
    {
        G.LocSystem.SetLaungie(LocSystem.LANG_RU);
        UpdateLan();
    }
    public void SetEn()
    {
        G.LocSystem.SetLaungie(LocSystem.LANG_EN);
        UpdateLan();
    }
    private void UpdateLan()
    {
        G.LocSystem.UpdateTexts();
    }

    public void LoadScene(string line)
    {
        G.SceneLoader.Load(line);
    }

    public void UpdatePanelFAQ()
    {
        G.faqPanel.UpdatePanelFAQ();
    }

    public void ShowSettingPanel()
    {
        G.pausePanel.UpdatePanels();
    }

    public void Quit()
    {
        Application.Quit();
    }

    public void StartMusic(AudioClip clip)
    {
        G.AudioManager.PlayMusic(clip);
    }

    public void SetModVolume(float volume)
    {
        modifVolume = volume;
    }

    public void PlaySound(AudioClip clip)
    {
        clip.PlayAsSound(modifVolume: modifVolume);
        modifVolume = 0;
    }

    public void StartGame()
    {
        G.configGame.GetConfigLevel().brain.StartLvl();
    }
    public void RestartGame()
    {
        if (G.mainEnterPoint != null)
            G.mainEnterPoint.StartCoroutine(G.mainEnterPoint.RestartGame());
    }

    public void CameraMovePos2()
    {
        G.mainEnterPoint.StartCoroutine(G.PlayerController.MoveAndRotate(G.PlayerController.position1, G.PlayerController.position2));
    }
    public void CameraMovePos1()
    {
        G.mainEnterPoint.StartCoroutine(G.PlayerController.MoveAndRotate(G.PlayerController.position2, G.PlayerController.position1));
    }
    public void Louse()
    {
        if(G.louse != null)
            G.louse.StartCoroutine(G.louse.Louses());
    }
    public void Win()
    {
        if (G.louse != null)
            G.louse.StartCoroutine(G.louse.Win());
    }

}
