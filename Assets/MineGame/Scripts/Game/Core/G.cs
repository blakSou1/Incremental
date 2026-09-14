using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class G
{
    public static bool IsPaused = false;

    public static ConfigGame configGame;

    public static RunState run;
    public static BoardVisualConfig boardVisualConfig;

    //Объекты в игре
    public static MainEnterPoint mainEnterPoint;
    public static EnemySprite enemySprite;
    public static EnemyHp enemyHp;
    public static PlayerController PlayerController;
    public static AI ai;
    public static Loss loss;
    public static Volume volume;
    public static PieceFactory pieceFactory;
    public static UIController UIController;
    public static DamageEnemyScenario DamageEnemyScenario;
    public static PiecePicker PiecePicker;
    public static CameraShake cameraShake;

    //обьекты не монобех контроллеры
    public static Inputs inputs;

    public static WinAndLoss winAndLouse;

    //Services
    public static AudioManager AudioManager;
    public static SceneLoader SceneLoader;
    public static PausePanel pausePanel;
    public static FAQ faqPanel;
}

public class ManagedBehaviour : MonoBehaviour
{
    void Update()
    {
        if (!G.IsPaused)
            PausableUpdate();
    }

    protected virtual void PausableUpdate()
    {
    }

    void FixedUpdate()
    {
        if (!G.IsPaused)
            PausableFixedUpdate();
    }

    protected virtual void PausableFixedUpdate()
    {
    }
}

public class RunState
{
    public float currentLevel = 0;

    public Status playerColor = Status.Black;

    //игрок берет из deck в hand, играет из hand
    public List<string> deck = new();
    public List<string> hand = new();
    public List<string> discardPile = new(); //использованные карты попадают сюда
    public List<string> trash = new();//использованные без возможности вернутся сюда

    //Health player
    public float Damage = 0;
    public float maxHealth = 10;

}