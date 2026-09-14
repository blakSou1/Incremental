using UnityEngine;

public class MainMenu : MonoBehaviour
{
    private void Awake()
    {
        G.AudioManager.PlayMusic(R.Audio.NewMainMenu);
    }
}
