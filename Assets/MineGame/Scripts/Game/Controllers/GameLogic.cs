using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class GameLogic
{
    [HideInInspector] public Status ActualColor = Status.Black;

    [HideInInspector] public InteractiveObject actualPieceInstance;

    public void Init()
    {
    }

    public IEnumerator InitStaticPieces()
    {
        Status stat = Status.None;

        int size = G.configGame.MatrixModel.matrixField.size;

        for (int i = 0; i < size; i++)
        {
            for (int s = 0; s < size; s++)
            {
                stat = G.configGame.MatrixModel.matrixField.data.GetValue(i, s).stat;
                if (stat == Status.None)
                    continue;

                InteractiveObject piece = G.pieceFactory.AddPiece(G.configGame.MatrixModel.matrixField.data.GetValue(i, s).idPiece);
                actualPieceInstance = piece;

                SpawnOnGrid(actualPieceInstance, G.mainEnterPoint.gridController.matrix.GetGrid(new(i, s)), G.configGame.MatrixModel.matrixField.data.GetValue(i, s).stat);

                yield return new WaitForSeconds(.15f);
            }
        }

        G.pieceFactory.AddPiece(ConfigGame.standardPiece);

        stat = Status.White;
    }

    public IEnumerator PlacePiece(GridBox grid)
    {
        if (grid == null || grid.indic == null) yield break;

        switch (G.mainEnterPoint.gameLogic.ActualColor)
        {
            case Status.Black:
                PlayerPlace(grid);
                break;

            case Status.White:
                EnemyPlace(grid);
                break;

            default:
                Debug.LogWarning("Unknown game state!");
                break;
        }
    }

    private void PlayerPlace(GridBox grid)
    {
        string pieceId = IsModifierPieces();

        SpawnPiece(pieceId);

        if (pieceId != ConfigGame.standardPiece)
            G.mainEnterPoint.modifierPieces.standartSlot.Click();

        G.mainEnterPoint.StartCoroutine(SpawnAndWaitAnimation(grid));

        actualPieceInstance.GetBaseModel().FlipOfPiece(grid.indic.revColorPieces);

        ActualColor = Status.White;
        (G.configGame.GetConfigLevel().brain as BoardBrain).isMove = false;
    }

    private void EnemyPlace(GridBox grid)
    {
        SpawnPiece(ConfigGame.standardPiece);

        G.mainEnterPoint.StartCoroutine(SpawnAndWaitAnimation(grid));

        actualPieceInstance.GetBaseModel().FlipOfPiece(grid.indic.revColorPieces);

        ActualColor = Status.Black;
        (G.configGame.GetConfigLevel().brain as BoardBrain).isMove = false;
    }

    private void SpawnPiece(string id)
    {
        actualPieceInstance = G.pieceFactory.AddPiece(id);
    }

    private IEnumerator SpawnAndWaitAnimation(GridBox grid)
    {
        bool isAnimationEnd = false;
        actualPieceInstance.animationController.endAnimation.AddListener(() => isAnimationEnd = true);

        SpawnOnGrid(actualPieceInstance, grid, ActualColor);

        while (!isAnimationEnd)
            yield return new WaitForEndOfFrame();
    }

    private string IsModifierPieces()
    {
        foreach (SlotModPiece slot in G.mainEnterPoint.modifierPieces.modSlots)
        {
            if (slot.piece == null || slot.piece.state.model.id == ConfigGame.standardPiece)
                continue;

            if (!slot.activ.activeInHierarchy)
                continue;

            InteractiveObject inter = slot.GetComponentInChildren<InteractiveObject>();
            string id = new(inter.state.model.id);

            G.run.hand.Remove(inter.GetBaseModel().id);
            GameObject.Destroy(inter.gameObject);

            slot.piece = null;

            return id;
        }

        return ConfigGame.standardPiece;
    }

    private void SpawnOnGrid(InteractiveObject piece, GridBox grid, Status color)
    {
        GameObject obj = piece.gameObject;
        piece.moveable.targetPosition = grid.transform.position;
        piece.transform.position = grid.transform.position;

        obj.name = grid.GetIndex().Item1 + " / " + grid.GetIndex().Item2;

        grid.SetStat(color);
        G.mainEnterPoint.StartCoroutine(grid.SetPiece(piece));
        piece.state.gridBox = grid;

        piece.SetColor(color);

        if (color == G.run.playerColor)
            G.mainEnterPoint.gridController.blackPieces.Add(piece);
        else G.mainEnterPoint.gridController.whitePieces.Add(piece);

        G.UIController.UpdateCountPlayers();
        G.UIController.motionText._textAnimator.ShowText("");

        G.AudioManager.PlaySound(R.Audio.SpawnPiece, 0, -.15f);
    }

}
