using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public static class G
{
    public static bool IsPaused = false;

    public static LocSystem LocSystem;
    public static AudioManager AudioManager;
    public static SceneLoader SceneLoader;
    public static PausePanel pausePanel;

    public static Inpyts inputs;

    public static ConfigGame configGame;

    public static RunState run;

    public static Interactor interactor;

    //Объекты в игре
    public static GameMode gameMode;
    public static EnemySprite enemySprite;
    public static GridFuncion gridFuncion;
    public static GameLogic gameLogic;
    public static PlayerController PlayerController;
    public static AI ai;
    public static WinAndLouse winAndLouse;
    public static Loss louse;
    public static Volume volume;
    public static ModifirePieces modifirePieces;
    public static Chooice chooice;

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
    public int level;
    public List<PieceState> pieceStorage = new(); //не сыгранные 
    public List<PieceBagState> pieceBag = new(); //в сумке игрока
    public int drawSize = 3;
    public int health = 10;
    public int maxHealth = 10;

    public bool HasPiece(string mID)
    {
        foreach (var db in pieceBag)
            if (db.id == mID)
                return true;
        return false;
    }
}