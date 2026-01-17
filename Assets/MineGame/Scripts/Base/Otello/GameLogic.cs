using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Index = System.Tuple<int, int>;

[Serializable]
public class GameLogic
{
    public TextThrower coinTextPlayer;
    public TextThrower coinTextEnemy;

    public Piece pieceObj;
    public Piece EnemyPieceObject;

    [HideInInspector] public Piece piece;
    
    private Transform parentPiece;

    public GameObject playerSelect;
    public GameObject EnemySelect;

    public void Init()
    {
        parentPiece = new GameObject("PiecePool").transform;

        playerSelect.SetActive(false);
        EnemySelect.SetActive(false);
    }

    public IEnumerator InitStaticPieces()
    {
        List<Index> centerCells = G.gridFuncion.GetCenterCells();
        GridBox.Status status = GridBox.Status.White;

        piece = G.configGame.standertPiece;

        foreach (var i in centerCells)
        {
            PlacePiece(status, G.gridFuncion.GetMatrix().GetGrid(i), true);
            status = (GridBox.Status)((int)status * -1);
            yield return new WaitForEndOfFrame();
        }

        if (G.PlayerController.playerColor == G.gameMode.playerColor)
        {
            playerSelect.SetActive(true);
            EnemySelect.SetActive(false);
        }
        else
        {
            EnemySelect.SetActive(true);
            playerSelect.SetActive(false);
        }

        PassTurn();
    }

    public void PlacePiece(GridBox.Status color, GridBox grid, bool isStatic = false)
    {
        if ((grid == null || grid.indic == null) && !isStatic) return;

        GameObject obj = GameObject.Instantiate(piece.gameObject, grid.transform.position, Quaternion.identity);

        obj.transform.parent = parentPiece;

        obj.name = grid.GetIndex().Item1 + " / " + grid.GetIndex().Item2;

        Piece piec = obj.GetComponent<Piece>();

        if (!isStatic)
            piec.FlipOfPiece(grid.indic.revColorPieces);

        grid.SetStat(color);
        grid.SetPiece(piec);

        piec.SetColor(color);

        if (color == G.gameMode.playerColor)
            G.gridFuncion.blackPieces.Add(piec);
        else G.gridFuncion.whitePieces.Add(piec);

        UpdateCountUI();
        G.gameMode.motionText.ThrowText(new LocString("", ""), R.normalVoice);

        if (isStatic) return;

        if (G.gameMode.playerColor == G.PlayerController.playerColor)
        {
            foreach(SlotModPiece i in G.modifirePieces.slots)
            {
                if(i.piece == piece && i.piece != G.configGame.standertPiece)
                {
                    G.configGame.piece.Remove(piece);
                    GameObject.Destroy(i.GetComponentInChildren<Piece>().gameObject);
                    i.piece = null;

                    List<SlotModPiece> standardSlots = G.modifirePieces.slots.Where(s => s is SlotModPieceStandart).ToList();

                    standardSlots[0].Click();
                }
            }
        }

        G.AudioManager.PlaySound(R.Audio.SpawnPiece, 0, -.15f);

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
            EnemySelect.SetActive(false);
            playerSelect.SetActive(true);

            G.gameMode.motionText.ThrowText(new LocString("Your move!", "Ваш ход!"), R.normalVoice);
        }
        else
        {
            if(!G.gridFuncion.ShowPossibleLocation((GridBox.Status)((int)G.gameMode.playerColor * -1)))
                return;

            G.gridFuncion.EnableAndDisableIndc(false);

            EnemySelect.SetActive(true);
            playerSelect.SetActive(false);

            G.gameMode.motionText.ThrowText(new LocString("The opponent's move!", "Ход противника!"), R.normalVoice);

            G.inputs.Player.Disable();
            G.gameMode.StartCoroutine(Next());
        }
    }
    IEnumerator Next()
    {
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.65f, 1.2f));
        G.ai.Execute(G.PlayerController.playerColor);

        G.inputs.Player.Enable();
    }

    public void ActualPiece()
    {
        piece = (G.PlayerController.playerColor == G.gameMode.playerColor) ? G.gameLogic.pieceObj : G.gameLogic.EnemyPieceObject;
    }

    private void UpdateCountUI()
    {
        string bText;
        string wText;

        if (G.gridFuncion.blackPieces.Count < 10)
            bText = "0" + G.gridFuncion.blackPieces.Count.ToString();
        else
            bText = G.gridFuncion.blackPieces.Count.ToString();

        if (G.gridFuncion.whitePieces.Count < 10)
            wText = "0" + G.gridFuncion.whitePieces.Count.ToString();
        else
            wText = G.gridFuncion.whitePieces.Count.ToString();

        coinTextPlayer.ThrowText(new LocString(bText, bText), R.normalVoice);
        coinTextEnemy.ThrowText(new LocString(wText, wText), R.normalVoice);
    }
}
