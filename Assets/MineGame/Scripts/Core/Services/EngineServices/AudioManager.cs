using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour, IService
{
    private AudioSource musicSource;
    private AudioSource soundsSource;

    public float musicVolume { get; private set; } = 0.3f;
    public float soundVolume { get; private set; } = 0.3f;
    private Dictionary<int, ActiveSoundLoop> _activeLoops = new();
    private int _nextLoopId = 0;

    private class ActiveSoundLoop
    {
        public Coroutine Coroutine { get; set; }
    }

    public void Init()
    {
        GameObject mSource = new("MusicSource") { transform = { parent = transform } };
        musicSource = mSource.AddComponent<AudioSource>();
        musicSource.loop = true;

        GameObject sSource = new("AudioSource") { transform = { parent = transform } };
        soundsSource = sSource.AddComponent<AudioSource>();
        soundsSource.loop = false;

        SetMusicVolume(musicVolume);
        SetSoundVolume(soundVolume);

        PlayMusic(R.Audio.NewMainMenu);
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        musicSource.volume = musicVolume;
    }

    public void SetSoundVolume(float value)
    {
        soundVolume = value;
        soundsSource.volume = soundVolume;
    }

    public void PlayMusic(AudioClip clip)
    {
        if (musicSource.clip == clip && musicSource.isPlaying) return;
        musicSource.clip = clip;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public GameObject PlaySound(AudioClip clip, float addedPitch, float modifVolume = 0)
    {
        if (clip == null) return null;
        GameObject tempAudioObject = new("TempAudio_" + clip.name);
        DontDestroyOnLoad(tempAudioObject);
        AudioSource audioSource = tempAudioObject.AddComponent<AudioSource>();

        audioSource.clip = clip;
        audioSource.volume = soundVolume + modifVolume;
        audioSource.pitch = 1f + addedPitch;
        audioSource.Play();

        Destroy(tempAudioObject, (clip.length / audioSource.pitch) + 0.1f);
        return tempAudioObject;
    }

    public int PlayLoop(AudioClip clip, float intervalSeconds, float deltaRandomPitch = 0)
    {
        if (clip == null) return -1;

        int id = _nextLoopId++;
        GameObject loopObject = new($"SoundLoop_{id}");
        loopObject.transform.parent = transform;

        ActiveSoundLoop loop = new();
        loop.Coroutine = StartCoroutine(SoundLoopCoroutine(clip, intervalSeconds, id, loopObject, deltaRandomPitch));
        _activeLoops.Add(id, loop);
        return id;
    }

    private IEnumerator SoundLoopCoroutine(AudioClip clip, float interval, int id, GameObject loopHolder, float deltaRandomPitch = 0)
    {
        GameObject soundHolder = null;

        while (true)
        {
            soundHolder = clip.PlayAsSoundRandomPitch(deltaRandomPitch);

            // Ждем интервал + длительность звука
            yield return new WaitForSeconds(interval + clip.length);
        }
    }

    public void RemoveLoop(int id)
    {
        if (_activeLoops.TryGetValue(id, out ActiveSoundLoop loop))
        {
            if (loop.Coroutine != null)
            {
                StopCoroutine(loop.Coroutine);
                loop.Coroutine = null;
            }
            _activeLoops.Remove(id);
        }
    }

    public void RemoveAllLoops()
    {
        foreach (var loop in _activeLoops.Values)
        {
            if (loop.Coroutine != null)
            {
                StopCoroutine(loop.Coroutine);
                loop.Coroutine = null;
            }
        }
        _activeLoops.Clear();
    }
}

public static class AudioExtensions
{
    public static void PlayAsSound(this AudioClip clip, float addedPitch = 0, float modifVolume = 0) =>
        G.AudioManager.PlaySound(clip, addedPitch, modifVolume);
    public static void PlayAsMusic(this AudioClip clip) =>
        G.AudioManager.PlayMusic(clip);
    public static GameObject PlayAsSoundRandomPitch(this AudioClip clip, float deltaPitch) =>
        G.AudioManager.PlaySound(clip, Random.Range(-deltaPitch, deltaPitch));
}

public class DecrementalDelayTimer
{
    private int minDelay;
    private float delayMultiplier;
    private int currentDelay;

    public DecrementalDelayTimer(int initDelay, int min, float multi)
    {
        minDelay = min;
        delayMultiplier = multi;
        currentDelay = initDelay;
    }

    public int GetDelay()
    {
        int result = currentDelay;
        currentDelay = Mathf.Max((int)(currentDelay * delayMultiplier), minDelay);
        return result;
    }
}
