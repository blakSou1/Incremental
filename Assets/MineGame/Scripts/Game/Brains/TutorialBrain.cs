using System.Collections;
using UnityEngine;

public class TutorialBrain : BoardBrain
{
    public override void StartLvl()
    {
        G.AudioManager.PlayMusic(R.Audio.tutorial);

        G.mainEnterPoint.gridController.NewMatrix();
        G.mainEnterPoint.StartCoroutine(StartAnimationSpawnGrid(G.configGame.MatrixModel.matrixField.size, 
            G.mainEnterPoint.gridController.matrix.GetData(), G.mainEnterPoint.gridController.matrix.GetParent()));
    }

    public override IEnumerator EndStartAnimation()
    {
        yield return G.mainEnterPoint.StartCoroutine(G.mainEnterPoint.gameLogic.InitStaticPieces());

        yield return new WaitForSeconds(.3f);

        G.mainEnterPoint.pieceController.StartInitModPiece();

        yield return new WaitForSeconds(.3f);

        G.mainEnterPoint.gridController.StartInitModGrid();

        PlaybleStartAnimationSpawnBoard = false;
    }

    protected override void EndTick()
    {
        if (!TotorialGridIsNull())
        {
            PlaybleStartAnimationSpawnBoard = true;
        }
    }

    protected override void StartMove()
    {
        int indicCount = 0;
        if (G.mainEnterPoint.gameLogic.actualPieceInstance != null)
            indicCount = G.mainEnterPoint.gridController.CreateIndisObject(
                G.mainEnterPoint.gameLogic.ActualColor,
                G.mainEnterPoint.gameLogic.actualPieceInstance);

        int whatIsMove = G.mainEnterPoint.conditionsOfVictoryAndDefeat.ShowPossibleLocation(indicCount);

        if (whatIsMove == 1)
        {
            PlaybleStartAnimationSpawnBoard = false;
            return;
        }
        else if (whatIsMove == 2)
        {
            G.mainEnterPoint.conditionsOfVictoryAndDefeat.Pass();
            return;
        }


        switch (G.mainEnterPoint.gameLogic.ActualColor)
        {
            case Status.Black:
                PlayerMove();
                break;

            case Status.White:
                EnemyMove();
                break;

            default:
                Debug.LogWarning("Unknown game state!");
                break;
        }
    }

    public IEnumerator RestartCurrentScene()
    {
        var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();

        yield return new WaitForSeconds(.8f);

        G.SceneLoader.Load(currentScene.name);
    }

    private bool TotorialGridIsNull()
    {
        int size = G.configGame.MatrixModel.matrixField.size;
        if (G.mainEnterPoint.gridController.matrix == null)
            return true;
        var data = G.mainEnterPoint.gridController.matrix.GetData();

        int count = 0;

        for (int i = 0; i < size; i++)
        {
            for (int s = 0; s < size; s++)
            {
                if (data[i, s] != null && data[i, s].GetStat() == Status.None && data[i, s].boxMode != null)
                    count++;
            }
        }

        return count > 0;
    }
}
