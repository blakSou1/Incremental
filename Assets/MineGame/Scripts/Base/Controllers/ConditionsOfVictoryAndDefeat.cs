using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConditionsOfVictoryAndDefeat
{
    private int item1
    {
        get { return G.configGridFunction.item1; }
    }

    public void Init()
    {
        G.conditionsOfVictoryAndDefeat = this;
    }

    /// <summary>
    /// Travel transfer 
    /// Changes the current player (from white to black and vice versa)
    /// Checks if the new player has available moves
    /// Shows indicators of possible moves
    /// Starts the AI ​​if it's the opponent's turn
    /// </summary>
    public void PassTurn()
    {
        G.PlayerController.playerColor = (GridBox.Status)((int)G.PlayerController.playerColor * -1);

        G.inputs.Player.Enable();

        if (G.PlayerController.playerColor == G.mainEnterPoint.playerColor)
        {
            if (!ShowPossibleLocation(G.mainEnterPoint.playerColor))
            {
                Pass();
                return;
            }

            G.gridController.CreateIndisObject(G.mainEnterPoint.playerColor, G.gameLogic.actualPieceInsanting);

            G.gridController.EnableAndDisableIndc(true);

            G.UIController.motionText.ThrowText(new LocString("Your move!", "Ваш ход!"), R.normalVoice);
        }
        else
        {
            if (!ShowPossibleLocation((GridBox.Status)((int)G.mainEnterPoint.playerColor * -1)))
            {
                Pass();
                return;
            }

            G.gridController.EnableAndDisableIndc(false);

            G.UIController.motionText.ThrowText(new LocString("The opponent's move!", "Ход противника!"), R.normalVoice);

            G.inputs.Player.Disable();
            G.mainEnterPoint.StartCoroutine(Next());
            static IEnumerator Next()
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.65f, 1.4f));
                G.ai.Execute(G.PlayerController.playerColor);
            }
        }

        G.UIController.ActualSelect();
    }

    /// <summary>
    /// Show possible moves
    /// Finds all the squares where a player of the specified color can move.
    /// Creates visual indicators(Indic) on these squares.
    /// Checks whether there are any available moves.
    /// If there are no moves, initiates a skip or ends the game.
    /// </summary>
    /// <param name="color"></param>
    /// <param name="isCheck"></param>
    /// <returns></returns>
    public bool ShowPossibleLocation(GridBox.Status color)//move possibility
    {
        int indicCount = G.gridController.CreateIndisObject(color, G.gameLogic.actualPieceInsanting);//PassTurn after PlacePiece

        if (indicCount == 0)
        {
            if (G.gridController.blackPieces.Count + G.gridController.whitePieces.Count == item1 * item1)
                G.mainEnterPoint.StartCoroutine(WhatWin());
            else if (G.gridController.blackPieces.Count == 0 || G.gridController.whitePieces.Count == 0)
                G.mainEnterPoint.StartCoroutine(WhatWin());

            return false;
        }

        return true;
    }//not move -> what win?

    private IEnumerator WhatWin()
    {
        yield return G.DamageEnemyScenario.StartCoroutine(G.DamageEnemyScenario.StartScenario());

        if (!G.enemyHp.WhatDead())
        {
            if (G.gridController.blackPieces.Count < G.gridController.whitePieces.Count)
                G.winAndLouse.WinEnemy();
            else if (G.gridController.blackPieces.Count > G.gridController.whitePieces.Count)
                G.winAndLouse.WinPlayer();
            else if (G.gridController.blackPieces.Count == G.gridController.whitePieces.Count)
                Drav();
        }
    }

    private void Drav()
    {
        G.UIController.IndicatorText("Draw");
        G.UIController.motionText.ThrowText(new LocString("Draw!", "Ничья!"), R.normalVoice);

        G.mainEnterPoint.isGameEnd = true;
    }//TODO

    public IEnumerator Pass()
    {
        G.UIController.IndicatorText("Pass");

        G.UIController.motionText.ThrowText(new LocString("No move!", "Нет хода!"), R.normalVoice);

        yield return new WaitForSeconds(1.3f);

        G.mainEnterPoint.disableInputForPass = true;

        PassTurn();

        yield return new WaitForSeconds(1.3f);

        G.UIController.IndicatorText("");
        G.mainEnterPoint.disableInputForPass = false;
    }

    /// <summary>
    /// Get possible moves
    /// </summary>
    /// <param name="color"></param>
    /// <param name="isCheck"></param>
    /// <returns></returns>
    public List<GridBox> GetPossibleLocation(GridBox.Status color, bool isCheck = false)
    {
        List<GridBox> list = G.gridController.indicPositionGrid;

        return list;
    }

}
