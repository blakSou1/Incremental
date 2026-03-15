using UnityEngine;

[CreateAssetMenu(fileName = "IHelper", menuName = "IHelper")]
public class IHelper : ScriptableObject
{
    float modifVolume = 0;

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
        if(G.gameMode != null)
            G.gameMode.StartGame();
    }
    public void RestartGame()
    {
        if (G.gameMode != null)
            G.gameMode.StartCoroutine(G.gameMode.RestartGame());
    }

    public void CameraMovePos2()
    {
        G.gameMode.StartCoroutine(G.PlayerController.MoveAndRotate(G.PlayerController.position1, G.PlayerController.position2));
    }
    public void CameraMovePos1()
    {
        G.gameMode.StartCoroutine(G.PlayerController.MoveAndRotate(G.PlayerController.position2, G.PlayerController.position1));
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
