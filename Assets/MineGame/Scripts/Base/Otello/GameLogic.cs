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

    public GameObject pieceObj;

    private Transform parentPiece;

    public GameObject playerSelect;
    public GameObject EnemySelect;

    public void Init()
    {
        parentPiece = new GameObject("PiecePool").transform;

        playerSelect.SetActive(false);
        EnemySelect.SetActive(false);

    }

    public void InitStaticPieces()
    {
        List<Index> centerCells = G.gridFuncion.GetCenterCells();
        GridBox.Status status = GridBox.Status.White;

        foreach (var i in centerCells)
        {
            PlacePiece(status, i, true);
            status = (GridBox.Status)((int)status * -1);
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
    }

    public void PlacePiece(GridBox.Status color, Index index, bool isStatic = false)
    {
        var grid = G.gridFuncion.GetMatrix().GetGrid(index);

        if (!CheckPieceValid(color, grid, out List<GridBox> dummy, true) && !isStatic) return;

        G.gridFuncion.ClearIndicObjs();

        Vector3 pos = G.gridFuncion.IndexToVector2(index);
        pos.z = -1;

        var obj = GameObject.Instantiate(pieceObj, pos, Quaternion.identity);
        obj.transform.parent = parentPiece;

        obj.name = index.Item1 + " / " + index.Item2;

        var piece = obj.GetComponent<Piece>();

        grid.SetStat(color);
        grid.SetPiece(piece);

        piece.SetColor(color);

        if (color == G.gameMode.playerColor)
            G.gridFuncion.blackPieces.Add(piece);
        else G.gridFuncion.whitePieces.Add(piece);

        UpdateCountUI();
        G.gameMode.motionText.ThrowText(new LocString("", ""), R.normalVoice);

        G.AudioManager.PlaySound(R.Audio.Clic05, 0);

        if (isStatic) return;

        PassTurn();
    }
    public void PlacePiece(GridBox.Status color, GridBox curentBox, bool isStatic = false)
    {
        if (!CheckPieceValid(color, curentBox, out List<GridBox> dummy, true) && !isStatic) return;

        G.gridFuncion.ClearIndicObjs();

        var obj = GameObject.Instantiate(pieceObj, curentBox.transform.position, Quaternion.identity);
        obj.transform.parent = parentPiece;

        obj.name = curentBox.GetIndex().Item1 + " / " + curentBox.GetIndex().Item2;

        var grid = curentBox;
        var piece = obj.GetComponent<Piece>();

        grid.SetStat(color);
        grid.SetPiece(piece);

        piece.SetColor(color);

        if (color == G.gameMode.playerColor)
            G.gridFuncion.blackPieces.Add(piece);
        else G.gridFuncion.whitePieces.Add(piece);

        UpdateCountUI();
        G.gameMode.motionText.ThrowText(new LocString("", ""), R.normalVoice);

        if (isStatic) return;

        PassTurn();
    }

    public void PassTurn()
    {
        G.PlayerController.playerColor = (GridBox.Status)((int)G.PlayerController.playerColor * -1);

        if (G.PlayerController.playerColor == G.gameMode.playerColor)
        {
            playerSelect.SetActive(true);
            EnemySelect.SetActive(false);

            G.gameMode.motionText.ThrowText(new LocString("Your move!", "Ваш ход!"), R.normalVoice);

            G.gridFuncion.ShowPossibleLocation(G.PlayerController.playerColor);
        }
        else
        {
            EnemySelect.SetActive(true);
            playerSelect.SetActive(false);

            G.gameMode.motionText.ThrowText(new LocString("The opponent's move!", "Ход противника!"), R.normalVoice);

            G.inputs.Player.Disable();
            G.gameMode.StartCoroutine(Next());

            IEnumerator Next()
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.65f, 1.2f));
                G.ai.Execute(GridBox.Status.White);

                G.inputs.Player.Enable();
            }
        }
    }

    public bool CheckPieceValid(GridBox.Status color, GridBox curentBox, out List<GridBox> revList, bool execute = false)
    {
        revList = new List<GridBox>();

        if (curentBox?.GetStat() != GridBox.Status.None)
            return false;

        var pieceList = G.gridFuncion.GetCrossPieces(curentBox.GetIndex());
        var sameColorPieces = pieceList.Where(piece => piece.GetStat() == color);
        var able = false;

        foreach (var piece in sameColorPieces)
        {
            if (G.gridFuncion.IsAdjacent(curentBox.GetIndex(), piece.GetIndex())) continue;

            var crossList = G.gridFuncion.GetCrossPieces(curentBox.GetIndex(), piece.GetIndex(), false);
            var revColorPieces = crossList.Where(p => p.GetStat() == (GridBox.Status)((int)color * -1));
            var b = crossList.Any(p => p.GetStat() == GridBox.Status.None || p.GetStat() == color);

            if (revColorPieces.Count() != 0 && !b)
            {
                if (execute)
                {
                    foreach (var revColorPiece in revColorPieces)
                    {
                        revColorPiece.Flip();
                        if (revColorPiece.GetStat() == GridBox.Status.Black)
                        {
                            G.gridFuncion.blackPieces.Add(revColorPiece.piece);
                            G.gridFuncion.whitePieces.Remove(revColorPiece.piece);
                        }
                        else
                        {
                            G.gridFuncion.whitePieces.Add(revColorPiece.piece);
                            G.gridFuncion.blackPieces.Remove(revColorPiece.piece);
                        }

                        UpdateCountUI();
                    }
                }

                revList.AddRange(revColorPieces);

                able = true;
            }
        }

        return able;
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
