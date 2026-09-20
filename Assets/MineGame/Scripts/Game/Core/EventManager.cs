using UnityEngine.Events;

public class EventManager
{
    public CoroutineHost host;

    public EventManager()
    {
        OnDestroyEvent();

        PlayerWin = new();
        PlayerLose = new();
        VolumeWeugth = new();
        CameraShake = new();
        UpdatePlayerInput = new();
        SetEnemyHp = new();
        Attack = new();
        DamagedPlayer = new();
    }

    public UnityEvent PlayerWin;
    public UnityEvent PlayerLose;
    public UnityEvent<float> VolumeWeugth;
    public UnityEvent<float> CameraShake;
    public UnityEvent<bool> UpdatePlayerInput;
    public UnityEvent<int> SetEnemyHp;
    public UnityEvent<int> Attack;
    public UnityEvent<int> DamagedPlayer;

    public void OnDestroyEvent()
    {
        PlayerWin?.RemoveAllListeners();
        PlayerLose?.RemoveAllListeners();
        VolumeWeugth?.RemoveAllListeners();
        CameraShake?.RemoveAllListeners();
        UpdatePlayerInput?.RemoveAllListeners();
        SetEnemyHp?.RemoveAllListeners();
        Attack?.RemoveAllListeners();
        DamagedPlayer?.RemoveAllListeners();
    }
}

