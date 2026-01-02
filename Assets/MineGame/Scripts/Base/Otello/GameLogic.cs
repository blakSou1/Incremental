using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Index = System.Tuple<int, int>;

[Serializable]
public class GameLogic
{
    public TextThrower coinTextPlayer;
    public TextThrower coinTextEnemy;

    public GameObject pieceObj;

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

        GameObject obj = GameObject.Instantiate(pieceObj, grid.transform.position, Quaternion.identity);
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

        G.AudioManager.PlaySound(R.Audio.SpawnPiece, 0, -.15f);

        PassTurn();
    }

    public void PassTurn()
    {
        G.PlayerController.playerColor = (GridBox.Status)((int)G.PlayerController.playerColor * -1);

        if (G.PlayerController.playerColor == G.gameMode.playerColor)
        {
            G.gridFuncion.ShowPossibleLocation(G.gameMode.playerColor);

            G.gridFuncion.EnableAndDisableIndc(true);
            playerSelect.SetActive(true);
            EnemySelect.SetActive(false);

            G.gameMode.motionText.ThrowText(new LocString("Your move!", "Ваш ход!"), R.normalVoice);
        }
        else
        {
            G.gridFuncion.ShowPossibleLocation((GridBox.Status)((int)G.gameMode.playerColor * -1));
            G.gridFuncion.EnableAndDisableIndc(false);

            EnemySelect.SetActive(true);
            playerSelect.SetActive(false);

            G.gameMode.motionText.ThrowText(new LocString("The opponent's move!", "Ход противника!"), R.normalVoice);

            G.inputs.Player.Disable();
            G.gameMode.StartCoroutine(Next());

            IEnumerator Next()
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.65f, 1.2f));
                G.ai.Execute((GridBox.Status)((int)G.gameMode.playerColor * -1));

                G.inputs.Player.Enable();
            }
        }
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
