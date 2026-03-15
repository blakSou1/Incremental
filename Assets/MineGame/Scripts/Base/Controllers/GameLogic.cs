using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Index = System.Tuple<int, int>;

[Serializable]
public class GameLogic
{
    [NonSerialized] public InteractiveObject pieceObj;
    [NonSerialized] public string EnemyPieceObject = ConfigGame.standertPiece;

    [HideInInspector] public string actualPiece;
    [HideInInspector] public InteractiveObject spawnPieceModel;

    private Transform parentPiece;

    public void Init()
    {
        parentPiece = new GameObject("PiecePool").transform;
    }

    public IEnumerator InitStaticPieces()
    {
        pieceObj = G.chooice.AddPiece(ConfigGame.standertPiece);

        List<Index> centerCells = G.gridFuncion.GetCenterCells();
        GridBox.Status status = GridBox.Status.White;

        actualPiece = ConfigGame.standertPiece;

        foreach (var i in centerCells)
        {
            G.gameMode.StartCoroutine(PlacePiece(status, G.gridFuncion.matrix.GetGrid(i), true));
            status = (GridBox.Status)((int)status * -1);
            yield return new WaitForEndOfFrame();
        }

        G.UIController.ActualSelect();

        yield return new WaitForSeconds(.5f);

        PassTurn();
    }

    public IEnumerator PlacePiece(GridBox.Status color, GridBox grid, bool isStatic = false)
    {
        if ((grid == null || grid.indic == null) && !isStatic) yield break;

        if (!isStatic && G.gameMode.playerColor == G.PlayerController.playerColor)
        {
            foreach (SlotModPiece i in G.modifirePieces.slots)
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

        spawnPieceModel = G.chooice.AddPiece(actualPiece);

        GameObject obj = spawnPieceModel.gameObject;
        spawnPieceModel.moveable.targetPosition = grid.transform.position;
        spawnPieceModel.transform.position = grid.transform.position;

        obj.transform.parent = parentPiece;

        obj.name = grid.GetIndex().Item1 + " / " + grid.GetIndex().Item2;

        grid.SetStat(color);
        Coroutine myCoroutine = G.gameMode.StartCoroutine(grid.SetPiece(spawnPieceModel));

        spawnPieceModel.SetColor(color);

        if (!isStatic)
        {
            if(pieceObj.state.model.id != ConfigGame.standertPiece && G.gameMode.playerColor == G.PlayerController.playerColor)
            {
                List<SlotModPiece> standardSlots = G.modifirePieces.slots.Where(s => s is SlotModPieceStandart).ToList();

                standardSlots[0].Click();
            }

            bool isNext = false;
            spawnPieceModel.animationController.endAnimation.AddListener(() => isNext = true);

            while (!isNext)
                yield return new WaitForEndOfFrame();

            spawnPieceModel.GetBaseModel().FlipOfPiece(grid.indic.revColorPieces);
        }

        if (color == G.gameMode.playerColor)
            G.gridFuncion.blackPieces.Add(spawnPieceModel);
        else G.gridFuncion.whitePieces.Add(spawnPieceModel);

        if (isStatic) yield break;

        G.UIController.UpdateCountPlayers();
        G.UIController.motionText._textAnimator.ShowText("");

        G.AudioManager.PlaySound(R.Audio.SpawnPiece, 0, -.15f);

        while (myCoroutine != null)
            yield return new WaitForSeconds(.2f);

        PassTurn();
    }

    public void PassTurn()
    {
        G.PlayerController.playerColor = (GridBox.Status)((int)G.PlayerController.playerColor * -1);

        G.gameLogic.ActualPiece();

        if (G.PlayerController.playerColor == G.gameMode.playerColor)
        {
            if(!G.gridFuncion.ShowPossibleLocation(G.gameMode.playerColor))
                return;

            G.gridFuncion.EnableAndDisableIndc(true);

            G.UIController.motionText.ThrowText(new LocString("Your move!", "Ваш ход!"), R.normalVoice);
        }
        else
        {
            if(!G.gridFuncion.ShowPossibleLocation((GridBox.Status)((int)G.gameMode.playerColor * -1)))
                return;

            G.gridFuncion.EnableAndDisableIndc(false);

            G.UIController.motionText.ThrowText(new LocString("The opponent's move!", "Ход противника!"), R.normalVoice);

            G.inputs.Player.Disable();
            G.gameMode.StartCoroutine(Next());
        }

        G.UIController.ActualSelect();
    }
    private IEnumerator Next()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.65f, 1.4f));
        G.ai.Execute(G.PlayerController.playerColor);

        G.inputs.Player.Enable();
    }

    public void ActualPiece()
    {
        actualPiece = (G.PlayerController.playerColor == G.gameMode.playerColor) ? pieceObj.state.model.id : EnemyPieceObject;
    }
}
