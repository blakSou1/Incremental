using System.Collections;
using UnityEngine;

public class GameMode : MonoBehaviour
{
    public GridBox.Status playerColor = GridBox.Status.Black;

    public GridFuncion gridFuncion;

    [HideInInspector] public bool disableInputForPass = false;
    [HideInInspector] public bool isGameEnd = false;
    [HideInInspector] public bool disableInputForSelect = false;

    private void Start()
    {
        InitComponents();
        StartGame();

        G.run.pieceStorage.Add("PieceMoveFull");
        G.run.pieceBag.Add("PieceMoveFull");

        G.run.pieceStorage.Add("PieceMoveFull");
        G.run.pieceBag.Add("PieceMoveFull");

        G.run.pieceStorage.Add("PieceMoveFull");
        G.run.pieceBag.Add("PieceMoveFull");//TODO
    }

    private void InitComponents()
    {
        G.gridFuncion = gridFuncion;
        G.modifirePieces = new();

        G.modifirePieces.Init();
        gridFuncion.Init();

        G.PlayerController.playerColor = (GridBox.Status)((int)playerColor * -1);
    }
    public void StartGame()
    {
        G.AudioManager.PlayMusic(R.Audio.tutorial);

        gridFuncion.InitIndexPos();
    }

    public void EndStartAnimation()
    {
        G.PlayerController.isStopped = false;

        G.ai.InitWeight();

        StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy()
    {
        yield return StartCoroutine(G.PlayerController.MoveAndRotate(G.PlayerController.position1, G.PlayerController.position2));

        G.enemySprite.UpdateSprite();

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(G.PlayerController.MoveAndRotate(G.PlayerController.position2, G.PlayerController.position1));

        yield return StartCoroutine(G.gameLogic.InitStaticPieces());

        yield return new WaitForSeconds(.3f);

        G.modifirePieces.SpawnPiece();

        yield return new WaitForSeconds(.3f);

        SpawnModifireGridBox();
    }

    private void SpawnModifireGridBox()
    {
        G.gridFuncion.matrix.GetGrid(new(1, 1)).SetModifire("Mod");
    }

    public void PlayerInputUpdate()
    {
        if (disableInputForPass || isGameEnd || disableInputForSelect)
            G.inputs.Player.Disable();
        else
            G.inputs.Player.Enable();
    }

    public IEnumerator RestartGame()
    {
        gridFuncion.ClearAllPieces();

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(G.gameLogic.InitStaticPieces());
    }

}