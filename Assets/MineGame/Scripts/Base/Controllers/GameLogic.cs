using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Index = System.Tuple<int, int>;

[Serializable]
public class GameLogic
{
    [NonSerialized] public InteractiveObject pieceObj;
    [NonSerialized] public string EnemyPieceObject = ConfigGame.standertPiece;

    [HideInInspector] public string actualPiece;
    [HideInInspector] public InteractiveObject actualPieceInsanting;

    private Transform parentPiece;

    public void Init()
    {
        parentPiece = new GameObject("PiecePool").transform;
    }

    public IEnumerator InitStaticPieces()
    {
        pieceObj = G.chooice.AddPiece(ConfigGame.standertPiece);

        List<Index> centerCells = G.gridController.GetCenterCells();
        GridBox.Status status = GridBox.Status.White;

        actualPiece = ConfigGame.standertPiece;

        foreach (var i in centerCells)
        {
            G.mainEnterPoint.StartCoroutine(PlacePiece(status, G.gridController.matrix.GetGrid(i), true));
            status = (GridBox.Status)((int)status * -1);
            yield return new WaitForEndOfFrame();
        }

        G.UIController.ActualSelect();

        yield return new WaitForSeconds(.5f);

        G.conditionsOfVictoryAndDefeat.PassTurn();
    }

    Coroutine myCoroutineSkillGridBox;
    public IEnumerator PlacePiece(GridBox.Status color, GridBox grid, bool isStatic = false)
    {
        if ((grid == null || grid.indic == null) && !isStatic) yield break;

        if (!isStatic && G.mainEnterPoint.playerColor == G.PlayerController.playerColor)
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

        if (!isStatic)
        {
            if(pieceObj.state.model.id != ConfigGame.standertPiece && G.mainEnterPoint.playerColor == G.PlayerController.playerColor)
                G.modifirePieces.standartSlot.Click();

            bool isNext = false;
            actualPieceInsanting.animationController.endAnimation.AddListener(() => isNext = true);

            while (!isNext)
                yield return new WaitForEndOfFrame();

            actualPieceInsanting.GetBaseModel().FlipOfPiece(grid.indic.revColorPieces);
        }

        if (color == G.mainEnterPoint.playerColor)
            G.gridController.blackPieces.Add(actualPieceInsanting);
        else G.gridController.whitePieces.Add(actualPieceInsanting);

        if (isStatic) yield break;

        G.UIController.UpdateCountPlayers();
        G.UIController.motionText._textAnimator.ShowText("");

        G.AudioManager.PlaySound(R.Audio.SpawnPiece, 0, -.15f);

        while (myCoroutineSkillGridBox != null)
            yield return new WaitForSeconds(.2f);

        G.conditionsOfVictoryAndDefeat.PassTurn();
    }
    public void DestroyMyCoroutineSkillGridBox()
    {
        myCoroutineSkillGridBox = null;
    }

    public void ActualPiece()
    {
        actualPiece = (G.PlayerController.playerColor == G.mainEnterPoint.playerColor) ? pieceObj.state.model.id : EnemyPieceObject;
    }
}
