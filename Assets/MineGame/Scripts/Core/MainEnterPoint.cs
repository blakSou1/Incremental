using System.Collections;
using UnityEngine;

public class MainEnterPoint : MonoBehaviour
{
    public Status playerColor = Status.Black;

    [HideInInspector] public bool disableInputForPass = false;
    [HideInInspector] public bool isGameEnd = false;
    [HideInInspector] public bool disableInputForSelect = false;

    BaseBrain brain;

    private void Start()
    {
        InitComponents();

        brain.StartLvl();
    }

    private void FixedUpdate()
    {
        brain.Tick();
    }

    private void InitComponents()
    {
        G.pieceController = new();
        G.gridController = new();
        G.modifirePieces = new();
        G.conditionsOfVictoryAndDefeat = new();
        G.gameLogic.Init();

        G.pieceController.Init();
        G.gridController.Init();
        G.modifirePieces.Init();
        G.conditionsOfVictoryAndDefeat.Init();

        brain = G.configGame.GetConfigLevel().brain;

        G.PlayerController.playerColor = (Status)((int)playerColor * -1);
    }
    

    //SoProgram
    public void PlayerInputUpdate()
    {
        if (disableInputForPass || isGameEnd || disableInputForSelect)
            G.inputs.Player.Disable();
        else
            G.inputs.Player.Enable();
    }

    public IEnumerator RestartGame()
    {
        G.gridController.ClearAllPieces();

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(G.gameLogic.InitStaticPieces());
    }

}