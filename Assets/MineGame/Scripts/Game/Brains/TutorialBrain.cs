using System.Collections;
using UnityEngine;

public class TutorialBrain : BoardBrain
{
    public override void StartLvl()
    {
        G.AudioManager.PlayMusic(R.Audio.tutorial);

        G.mainEnterPoint.StartCoroutine(G.UIController.FadeCanvasGroup(G.UIController.GroupTextActualLvl, 0f, 0f));

        G.mainEnterPoint.gridController.NewMatrix();
        G.mainEnterPoint.StartCoroutine(StartAnimationSpawnGrid(G.configGame.MatrixModel.matrixField.size, G.mainEnterPoint.gridController.matrix.GetData(), G.mainEnterPoint.gridController.matrix.GetParent()));
    }

    public override IEnumerator EndStartAnimation()
    {
        G.PlayerController.isStopped = false;

        //yield return G.mainEnterPoint.StartCoroutine(SpawnEnemy());

        yield return G.mainEnterPoint.StartCoroutine(G.mainEnterPoint.gameLogic.InitStaticPieces());

        yield return new WaitForSeconds(.3f);

        G.mainEnterPoint.pieceController.StartInitModPiece();

        yield return new WaitForSeconds(.3f);

        G.mainEnterPoint.gridController.StartInitModGrid();

        isMove = false;
        startIsEnd = true;
    }

    protected override void EndTick()
    {
        if (!TotorialGridIsNull())
        {
            (G.configGame.GetConfigLevel().matrixNode as TutorialNode).NextIndex();

            G.mainEnterPoint.StartCoroutine(RestartCurrentScene());

            startIsEnd = false;
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
