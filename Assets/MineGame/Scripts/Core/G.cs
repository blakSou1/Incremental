using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class G
{
    public static bool IsPaused = false;

    public static ConfigGame configGame;

    public static RunState run;
    public static ConfigGridFunction configGridFunction;

    //Объекты в игре
    public static MainEnterPoint mainEnterPoint;
    public static EnemySprite enemySprite;
    public static EnemyHp enemyHp;
    public static GameLogic gameLogic;
    public static PlayerController PlayerController;
    public static AI ai;
    public static Loss louse;
    public static Volume volume;
    public static ModifirePieces modifirePieces;
    public static Chooice chooice;
    public static UIController UIController;
    public static DamageEnemyScenario DamageEnemyScenario;
    public static PiecePicker PiecePicker;
    public static CameraShake cameraShake;

    //обьекты не монобех контроллеры
    public static Inpyts inputs;

    public static LocSystem LocSystem;
    public static AudioManager AudioManager;
    public static SceneLoader SceneLoader;
    public static PausePanel pausePanel;
    public static FAQ faqPanel;

    public static WinAndLouse winAndLouse;
    public static PieceController pieceController;
    public static GridController gridController;
    public static ConditionsOfVictoryAndDefeat conditionsOfVictoryAndDefeat;

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
    public float indexLvl = 0;
    public List<string> pieceStorage = new(); //не сыгранные 
    public List<string> pieceBag = new(); //в сумке игрока
    public float Damage = 0;
    public float maxHealth = 10;

    public bool HasPiece(string mID)
    {
        foreach (var db in pieceBag)
            if (db == mID)
                return true;
        return false;
    }
}