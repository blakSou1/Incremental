using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConditionsOfVictoryAndDefeat
{
    private int size
    {
        get { return G.configGame.MatrixModel.matrixField.size; }
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
    public int ShowPossibleLocation(int indicCount)//move possibility
    {
        if (indicCount == 0)
        {
            if (G.mainEnterPoint.gridController.blackPieces.Count + G.mainEnterPoint.gridController.whitePieces.Count == size * size)
            {
                G.mainEnterPoint.StartCoroutine(WhatWin());
                return 1;
            }
            else if (G.mainEnterPoint.gridController.blackPieces.Count == 0 || G.mainEnterPoint.gridController.whitePieces.Count == 0)
            {
                G.mainEnterPoint.StartCoroutine(WhatWin());
                return 1;
            }

            return 2;
        }

        return 3;
    }//not move -> what win?

    private IEnumerator WhatWin()
    {
        yield return G.DamageEnemyScenario.StartCoroutine(G.DamageEnemyScenario.StartScenario());

        if (!G.enemyHp.WhatDead())
        {
            if (G.mainEnterPoint.gridController.blackPieces.Count < G.mainEnterPoint.gridController.whitePieces.Count)
                G.eventManager.PlayerLose.Invoke();
            else if (G.mainEnterPoint.gridController.blackPieces.Count > G.mainEnterPoint.gridController.whitePieces.Count)
                G.eventManager.PlayerWin.Invoke();
            else if (G.mainEnterPoint.gridController.blackPieces.Count == G.mainEnterPoint.gridController.whitePieces.Count)
                Draw();
        }
    }

    private void Draw()
    {
        G.UIController.IndicatorText("Draw");
        G.UIController.motionText.ThrowText("Draw!", R.normalVoice);

        G.eventManager.UpdatePlayerInput.Invoke(true);
    }//TODO

    public IEnumerator Pass()
    {
        G.UIController.IndicatorText("Pass");

        G.UIController.motionText.ThrowText("No move!", R.normalVoice);

        yield return new WaitForSeconds(1.3f);

        G.eventManager.UpdatePlayerInput.Invoke(false);

        yield return new WaitForSeconds(1.3f);

        G.UIController.IndicatorText("");
        G.eventManager.UpdatePlayerInput.Invoke(true);////////////////////
    }

    /// <summary>
    /// Get possible moves
    /// </summary>
    /// <returns></returns>
    public List<GridBox> GetPossibleLocation()
    {
        List<GridBox> list = G.mainEnterPoint.gridController.indicPositionGrid;

        return list;
    }

}
