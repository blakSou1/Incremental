using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderSettings : MonoBehaviour
{
    private Slider slider;

    public enum VolumeType { Sound, Music }
    public VolumeType volumeType = VolumeType.Sound;

    public void Start()
    {
        slider = GetComponent<Slider>();
        if (volumeType == VolumeType.Sound)
            slider.value = G.AudioManager.soundVolume;
        else
            slider.value = G.AudioManager.musicVolume;
    }

    public void UpdateMusicVolume()
    {
        G.AudioManager.SetMusicVolume(slider.value);
    }

    public void UpdateSoundVolume()
    {
        G.AudioManager.SetSoundVolume(slider.value);
    }
}
