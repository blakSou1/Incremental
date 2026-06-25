using System;
using System.Collections;
using UnityEngine;

[Serializable]
public class GameLogic
{
    [NonSerialized] public InteractiveObject pieceObj;
    [NonSerialized] public string EnemyPieceObject = ConfigGame.standertPiece;

    [HideInInspector] public string actualPiece;
    [HideInInspector] public InteractiveObject actualPieceInsanting;

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

                CreatePieceProcedure(G.configGame.MatrixModel.matrixField.data.GetValue(i, s).idPiece, G.gridController.matrix.GetGrid(new (i, s)), G.configGame.MatrixModel.matrixField.data.GetValue(i, s).stat);

                yield return new WaitForSeconds(.15f);
            }
        }

        pieceObj = G.chooice.AddPiece(ConfigGame.standertPiece);

        stat = Status.White;

        actualPiece = ConfigGame.standertPiece;

        G.UIController.ActualSelect();
    }

    Coroutine myCoroutineSkillGridBox;
    public IEnumerator PlacePiece(Status color, GridBox grid)
    {
        if (grid == null || grid.indic == null) yield break;

        if (G.mainEnterPoint.playerColor == G.PlayerController.playerColor)
        {
            foreach (SlotModPiece i in G.modifirePieces.modSlots)
            {
                if (i.piece == null || i.piece.state.model.id == ConfigGame.standertPiece) continue;

                if (i.activ.activeInHierarchy)
                {
                    InteractiveObject inter = i.GetComponentInChildren<InteractiveObject>();
                    actualPiece = inter.state.model.id;

                    G.run.pieceStorage.Remove(inter.GetBaseModel().id);
                    GameObject.Destroy(inter.gameObject);

                    i.piece = null;
                }
            }
        }

        G.gameLogic.ActualPiece();
        actualPieceInsanting = G.chooice.AddPiece(actualPiece);

        GameObject obj = actualPieceInsanting.gameObject;
        actualPieceInsanting.moveable.targetPosition = grid.transform.position;
        actualPieceInsanting.transform.position = grid.transform.position;

        obj.transform.parent = parentPiece;

        obj.name = grid.GetIndex().Item1 + " / " + grid.GetIndex().Item2;

        grid.SetStat(color);
        myCoroutineSkillGridBox = G.mainEnterPoint.StartCoroutine(grid.SetPiece(actualPieceInsanting));
        actualPieceInsanting.state.gridBox = grid;

        actualPieceInsanting.SetColor(color);

        if(pieceObj.state.model.id != ConfigGame.standertPiece && G.mainEnterPoint.playerColor == G.PlayerController.playerColor)
            G.modifirePieces.standartSlot.Click();

        bool isNext = false;
        actualPieceInsanting.animationController.endAnimation.AddListener(() => isNext = true);

        while (!isNext)
            yield return new WaitForEndOfFrame();

        actualPieceInsanting.GetBaseModel().FlipOfPiece(grid.indic.revColorPieces);

        if (color == G.mainEnterPoint.playerColor)
            G.gridController.blackPieces.Add(actualPieceInsanting);
        else G.gridController.whitePieces.Add(actualPieceInsanting);

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
        InteractiveObject piece = G.chooice.AddPiece(id);
        actualPieceInsanting = piece;

        GameObject obj = piece.gameObject;
        piece.moveable.targetPosition = grid.transform.position;
        piece.transform.position = grid.transform.position;

        obj.transform.parent = parentPiece;

        obj.name = grid.GetIndex().Item1 + " / " + grid.GetIndex().Item2;

        grid.SetStat(color);
        myCoroutineSkillGridBox = G.mainEnterPoint.StartCoroutine(grid.SetPiece(piece));
        piece.state.gridBox = grid;

        piece.SetColor(color);

        if (color == G.mainEnterPoint.playerColor)
            G.gridController.blackPieces.Add(piece);
        else G.gridController.whitePieces.Add(piece);

        G.UIController.UpdateCountPlayers();
        G.UIController.motionText._textAnimator.ShowText("");

        G.AudioManager.PlaySound(R.Audio.SpawnPiece, 0, -.15f);

        return piece;
    }

    public void ActualPiece()
    {
        actualPiece = (G.PlayerController.playerColor == G.mainEnterPoint.playerColor) ? pieceObj.state.model.id : EnemyPieceObject;
    }
}
