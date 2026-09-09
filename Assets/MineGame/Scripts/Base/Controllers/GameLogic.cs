using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class GameLogic
{
    [NonSerialized] public InteractiveObject pieceObj;
    [NonSerialized] public string EnemyPieceObject = ConfigGame.standardPiece;

    [HideInInspector] public string actualPiece;
    [HideInInspector] public InteractiveObject actualPieceInstance;

    private Transform parentPiece;

    public bool isPlace = false;

    public void Init()
    {
        parentPiece = new GameObject("PiecePool").transform;
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

                CreatePieceProcedure(G.configGame.MatrixModel.matrixField.data.GetValue(i, s).idPiece, G.mainEnterPoint.gridController.matrix.GetGrid(new (i, s)), G.configGame.MatrixModel.matrixField.data.GetValue(i, s).stat);

                yield return new WaitForSeconds(.15f);
            }
        }

        pieceObj = G.choice.AddPiece(ConfigGame.standardPiece);

        stat = Status.White;

        actualPiece = ConfigGame.standardPiece;

        G.UIController.ActualSelect();
    }

    Coroutine myCoroutineSkillGridBox;
    public IEnumerator PlacePiece(Status color, GridBox grid)
    {
        if (grid == null || grid.indic == null) yield break;

        if (G.run.playerColor == G.PlayerController.playerColor)
        {
            foreach (SlotModPiece i in G.mainEnterPoint.modifierPieces.modSlots)
            {
                if (i.piece == null || i.piece.state.model.id == ConfigGame.standardPiece) continue;

                if (i.activ.activeInHierarchy)
                {
                    InteractiveObject inter = i.GetComponentInChildren<InteractiveObject>();
                    actualPiece = inter.state.model.id;

                    G.run.hand.Remove(inter.GetBaseModel().id);
                    GameObject.Destroy(inter.gameObject);

                    i.piece = null;
                }
            }
        }

        G.mainEnterPoint.gameLogic.ActualPiece();
        actualPieceInstance = G.choice.AddPiece(actualPiece);

        GameObject obj = actualPieceInstance.gameObject;
        actualPieceInstance.moveable.targetPosition = grid.transform.position;
        actualPieceInstance.transform.position = grid.transform.position;

        obj.transform.parent = parentPiece;

        obj.name = grid.GetIndex().Item1 + " / " + grid.GetIndex().Item2;

        grid.SetStat(color);
        myCoroutineSkillGridBox = G.mainEnterPoint.StartCoroutine(grid.SetPiece(actualPieceInstance));
        actualPieceInstance.state.gridBox = grid;

        actualPieceInstance.SetColor(color);

        if(pieceObj.state.model.id != ConfigGame.standardPiece && G.run.playerColor == G.PlayerController.playerColor)
            G.mainEnterPoint.modifierPieces.standartSlot.Click();

        bool isNext = false;
        actualPieceInstance.animationController.endAnimation.AddListener(() => isNext = true);

        while (!isNext)
            yield return new WaitForEndOfFrame();

        actualPieceInstance.GetBaseModel().FlipOfPiece(grid.indic.revColorPieces);

        if (color == G.run.playerColor)
            G.mainEnterPoint.gridController.blackPieces.Add(actualPieceInstance);
        else G.mainEnterPoint.gridController.whitePieces.Add(actualPieceInstance);

        G.UIController.UpdateCountPlayers();
        G.UIController.motionText._textAnimator.ShowText("");

        G.AudioManager.PlaySound(R.Audio.SpawnPiece, 0, -.15f);

        while (myCoroutineSkillGridBox != null)
            yield return new WaitForSeconds(.2f);

        isPlace = true;
    }

    public void DestroyMyCoroutineSkillGridBox()
    {
        myCoroutineSkillGridBox = null;
    }

    private InteractiveObject CreatePieceProcedure(string id, GridBox grid, Status color)
    {
        InteractiveObject piece = G.choice.AddPiece(id);
        actualPieceInstance = piece;

        GameObject obj = piece.gameObject;
        piece.moveable.targetPosition = grid.transform.position;
        piece.transform.position = grid.transform.position;

        obj.transform.parent = parentPiece;

        obj.name = grid.GetIndex().Item1 + " / " + grid.GetIndex().Item2;

        grid.SetStat(color);
        myCoroutineSkillGridBox = G.mainEnterPoint.StartCoroutine(grid.SetPiece(piece));
        piece.state.gridBox = grid;

        piece.SetColor(color);

        if (color == G.run.playerColor)
            G.mainEnterPoint.gridController.blackPieces.Add(piece);
        else G.mainEnterPoint.gridController.whitePieces.Add(piece);

        G.UIController.UpdateCountPlayers();
        G.UIController.motionText._textAnimator.ShowText("");

        G.AudioManager.PlaySound(R.Audio.SpawnPiece, 0, -.15f);

        return piece;
    }

    public void ActualPiece()
    {
        actualPiece = (G.PlayerController.playerColor == G.run.playerColor) ? pieceObj.state.model.id : EnemyPieceObject;
    }
}
