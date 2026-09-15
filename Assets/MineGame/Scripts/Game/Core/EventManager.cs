using UnityEngine.Events;

public class EventManager
{
    public EventManager()
    {
        OnDestroyEvent();
        PlayerWin = new();
        PlayerLose = new();
        VolumeWeugth = new();
        CameraShake = new();
        UpdatePlayerInput = new();
    }

    public UnityEvent PlayerWin;
    public UnityEvent PlayerLose;
    public UnityEvent<float> VolumeWeugth;
    public UnityEvent<float> CameraShake;
    public UnityEvent<bool> UpdatePlayerInput;

    public void OnDestroyEvent()
    {
        PlayerWin?.RemoveAllListeners();
        PlayerLose?.RemoveAllListeners();
        VolumeWeugth?.RemoveAllListeners();
        CameraShake?.RemoveAllListeners();
        UpdatePlayerInput?.RemoveAllListeners();
    }
}
