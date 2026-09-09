using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConditionsOfVictoryAndDefeat
{
    private int item1
    {
        get { return G.configGame.MatrixModel.matrixField.size; }
    }

    public void Init()
    {
        G.mainEnterPoint.conditionsOfVictoryAndDefeat = this;
    }

    public void PlayerMove()
    {
        G.inputs.Player.Enable();

        G.mainEnterPoint.gridController.EnableAndDisableIndc(true);

        G.UIController.motionText.ThrowText("Your move!", R.normalVoice);

    }
    public void EnemyMove()
    {
        G.inputs.Player.Disable();

        G.mainEnterPoint.gridController.EnableAndDisableIndc(false);

        G.UIController.motionText.ThrowText("The opponent's move!", R.normalVoice);

        G.mainEnterPoint.StartCoroutine(Next());
        static IEnumerator Next()
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.65f, 1.4f));
            G.ai.Execute(G.PlayerController.playerColor);
        }

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
    public bool ShowPossibleLocation(int indicCount)//move possibility
    {
        if (indicCount == 0)
        {
            if (G.mainEnterPoint.gridController.blackPieces.Count + G.mainEnterPoint.gridController.whitePieces.Count == item1 * item1)
                G.mainEnterPoint.StartCoroutine(WhatWin());
            else if (G.mainEnterPoint.gridController.blackPieces.Count == 0 || G.mainEnterPoint.gridController.whitePieces.Count == 0)
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
            if (G.mainEnterPoint.gridController.blackPieces.Count < G.mainEnterPoint.gridController.whitePieces.Count)
                G.winAndLouse.WinEnemy();
            else if (G.mainEnterPoint.gridController.blackPieces.Count > G.mainEnterPoint.gridController.whitePieces.Count)
                G.winAndLouse.WinPlayer();
            else if (G.mainEnterPoint.gridController.blackPieces.Count == G.mainEnterPoint.gridController.whitePieces.Count)
                Draw();
        }
    }

    private void Draw()
    {
        G.UIController.IndicatorText("Draw");
        G.UIController.motionText.ThrowText("Draw!", R.normalVoice);

        G.mainEnterPoint.PlayerInputUpdate(true);
    }//TODO

    public IEnumerator Pass()
    {
        G.UIController.IndicatorText("Pass");

        G.UIController.motionText.ThrowText("No move!", R.normalVoice);

        yield return new WaitForSeconds(1.3f);

        G.mainEnterPoint.PlayerInputUpdate(false);

        G.mainEnterPoint.gameLogic.isPlace = false;

        yield return new WaitForSeconds(1.3f);

        G.UIController.IndicatorText("");
        G.mainEnterPoint.PlayerInputUpdate(true);
    }

    /// <summary>
    /// Get possible moves
    /// </summary>
    /// <param name="color"></param>
    /// <param name="isCheck"></param>
    /// <returns></returns>
    public List<GridBox> GetPossibleLocation(Status color, bool isCheck = false)
    {
        List<GridBox> list = G.mainEnterPoint.gridController.indicPositionGrid;

        return list;
    }

}
