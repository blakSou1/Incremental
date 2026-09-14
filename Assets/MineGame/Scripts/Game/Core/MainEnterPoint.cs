using System.Collections;
using UnityEngine;

public class MainEnterPoint : ManagedBehaviour
{
    public PieceController pieceController;
    public GridController gridController;
    public ModifirePieces modifierPieces;
    public ConditionsOfVictoryAndDefeat conditionsOfVictoryAndDefeat;
    public GameLogic gameLogic;

    BaseBrain brain;

    private void Start()
    {
        InitComponents();

        brain.StartLvl();
    }

    protected override void PausableFixedUpdate()
    {
        brain.Tick();
    }

    private void InitComponents()
    {
        pieceController = new();
        gridController = new();
        modifierPieces = new();
        conditionsOfVictoryAndDefeat = new();
        gameLogic = new();

        pieceController.Init();
        gridController.Init();
        modifierPieces.Init();
        conditionsOfVictoryAndDefeat.Init();
        gameLogic.Init();

        brain = G.configGame.GetConfigLevel().brain;
    }

    public void PlayerInputUpdate(bool isEnablePlayerInput = true)
    {
        if (isEnablePlayerInput)
            G.inputs.Player.Enable();
        else
            G.inputs.Player.Disable();
    }

    public IEnumerator RestartGame()
    {
        gridController.ClearAllPieces();

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(gameLogic.InitStaticPieces());
    }

}