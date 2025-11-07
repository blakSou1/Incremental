using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using Cysharp.Threading.Tasks;

public class AudioManager : MonoBehaviour, IService
{
    private AudioSource musicSource;
    private AudioSource soundsSource;

    public float musicVolume { get; private set; } = 0.9f;
    public float soundVolume { get; private set; } = 0.5f;
    private Dictionary<int, ActiveSoundLoop> _activeLoops = new();
    private int _nextLoopId = 0;

    private class ActiveSoundLoop
    {
        public CancellationTokenSource Cts { get; set; }
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

        PlayMusic(R.Audio.MainMenu);
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
    
    public GameObject PlaySound(AudioClip clip, float addedPitch) {
        if (clip == null) return null;
        GameObject tempAudioObject = new("TempAudio_" + clip.name);
        DontDestroyOnLoad(tempAudioObject);
        AudioSource audioSource = tempAudioObject.AddComponent<AudioSource>();

        audioSource.clip = clip;
        audioSource.volume = soundVolume;
        audioSource.pitch = 1f + addedPitch;
        audioSource.Play();
        
        Destroy(tempAudioObject, (clip.length / audioSource.pitch) + 0.1f);
        return tempAudioObject;
    }

    public int PlayLoop(AudioClip clip, float intervalSeconds, float deltaRandomPitch = 0)
    {
        if (clip == null) return -1;

        int id = _nextLoopId++;
        var cts = new CancellationTokenSource();

        GameObject loopObject = new($"SoundLoop_{id}");
        loopObject.transform.parent = transform;

        ActiveSoundLoop loop = new()
        {
            Cts = cts
        };
        _activeLoops.Add(id, loop);
        SoundLoopTask(clip, intervalSeconds, id, cts.Token, loopObject).Forget();
        return id;
    }
    
    private async UniTaskVoid SoundLoopTask(AudioClip clip, float interval, int id, CancellationToken ct, GameObject loopHolder, float deltarandomPitch = 0)
    {
        GameObject soundHolder = null;
        try
        {
            while (!ct.IsCancellationRequested)
            {
                soundHolder = clip.PlayAsSoundRandomPitch(deltarandomPitch);
                
                // Ждем интервал + длительность звука с возможностью отмены
                await UniTask.Delay(
                    (int)((interval + clip.length) * 1000), 
                    DelayType.DeltaTime, 
                    PlayerLoopTiming.Update, 
                    ct);
            }
        }
        catch
        {
            if (_activeLoops.TryGetValue(id, out var loop))
            {
                _activeLoops.Remove(id);
                if(soundHolder != null) Destroy(soundHolder);
            }
        }
    }
    public void RemoveLoop(int id)
    {
        if (_activeLoops.TryGetValue(id, out ActiveSoundLoop loop))
        {
            loop.Cts?.Cancel();
            loop.Cts?.Dispose();
        }
    }
    
    public void RemoveAllLoops()
    {
        foreach (var loop in _activeLoops.Values)
        {
            loop.Cts?.Cancel();
            loop.Cts?.Dispose();
        }
        _activeLoops.Clear();
    }
}

public static class AudioExtensions
{
    public static void PlayAsSound(this AudioClip clip, float addedPitch = 0) =>
        G.AudioManager.PlaySound(clip, addedPitch);
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