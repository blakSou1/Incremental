using UnityEngine;

[CreateAssetMenu(fileName = "AudioHelper", menuName = "Helpers/Audio")]
public class AudioHelper : ScriptableObject
{
    private float volumeModifier = 0f;

    public void SetVolumeModifier(float modifier)
    {
        volumeModifier = Mathf.Clamp(modifier, -80f, 20f);
    }

    public void PlaySound(AudioClip clip)
    {
        if (clip == null) return;
        clip.PlayAsSound(volumeModifier);
        volumeModifier = 0f;
    }

    public void StartMusic(AudioClip clip)
    {
        G.AudioManager.PlayMusic(clip);
    }
}
