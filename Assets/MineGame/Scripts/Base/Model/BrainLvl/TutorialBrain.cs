using DG.Tweening;
using System.Collections;
using UnityEngine;

public class TutorialBrain : BaseBrain
{
    private bool isMove = false;

    public override void StartLvl()
    {
        G.AudioManager.PlayMusic(R.Audio.tutorial);

        G.mainEnterPoint.StartCoroutine(G.UIController.FadeCanvasGroup(G.UIController.GroupTextActualLvl, 0f, 0f));

        G.gridController.NewMatrix();
        G.mainEnterPoint.StartCoroutine(StartAnimationSpawnGrid(G.configGame.MatrixModel.matrixField.size, G.gridController.matrix.GetData(), G.gridController.matrix.GetParent()));
    }

    private IEnumerator StartAnimationSpawnGrid(int index, GridBox[,] data, Transform parent)
    {
        yield return G.mainEnterPoint.StartCoroutine(VisibalTextActualLvl((G.configGame.GetConfigLevel().matrixNode.GetMatrix() as ParamT).text.ToString()));

        float offsetX = (index - 1) * G.configGridFunction.indentGrid.x / 2;
        float offsetY = (index - 1) * G.configGridFunction.indentGrid.y / 2;

        System.Random random = new();
        bool isAudi = false;

        for (int i = 0; i < index; i++)
        {
            for (int j = 0; j < index; j++)
            {
                int randomIndex = random.Next(1, 4);

                data[i, j] = GameObject.Instantiate(G.configGridFunction.prefabGridBox, parent);
                data[i, j].transform.position = new(G.configGridFunction.indentGrid.x * i - offsetX, G.configGridFunction.indentGrid.y * j - offsetY);

                data[i, j].SetIndex(i, j);

                Vector3 scale = data[i, j].transform.localScale;

                data[i, j].transform.localScale = Vector3.zero;
                Tween tween = data[i, j].transform.DOScale(scale, 0.3f).SetEase(Ease.OutBounce);

                yield return new WaitForSeconds(.035f);

                if (isAudi)
                {
                    isAudi = false;
                    continue;
                }

                switch (randomIndex)
                {
                    case 1:
                        G.AudioManager.PlaySound(R.Audio.pop1, -.05f);
                        break;
                    case 2:
                        G.AudioManager.PlaySound(R.Audio.pop2, -.07f);
                        break;
                    case 3:
                        G.AudioManager.PlaySound(R.Audio.pop3, 0);
                        break;
                }

                isAudi = true;
            }
        }

        G.ai.InitWeight();

        G.mainEnterPoint.StartCoroutine(EndStartAnimation());
    }

    public IEnumerator EndStartAnimation()
    {
        G.PlayerController.isStopped = false;

        //yield return G.mainEnterPoint.StartCoroutine(SpawnEnemy());

        yield return G.mainEnterPoint.StartCoroutine(G.gameLogic.InitStaticPieces());

        yield return new WaitForSeconds(.3f);

        G.pieceController.StartInitModPiece();

        yield return new WaitForSeconds(.3f);

        G.gridController.StartInitModGrid();

        isMove = false;
        startIsEnd = true;
    }

    public override void Tick()
    {
        if (!startIsEnd)
            return;

        if (!isMove)
        {
            G.PlayerController.playerColor = (Status)((int)G.PlayerController.playerColor * -1);

            int indicCount = 0;
            if (G.gameLogic.actualPieceInsanting != null)
                indicCount = G.gridController.CreateIndisObject(G.PlayerController.playerColor, G.gameLogic.actualPieceInsanting);

            if (G.PlayerController.playerColor == G.mainEnterPoint.playerColor)
                G.conditionsOfVictoryAndDefeat.PlayerMove();
            else
            {
                if (!G.conditionsOfVictoryAndDefeat.ShowPossibleLocation(indicCount))
                {
                    G.conditionsOfVictoryAndDefeat.Pass();
                    return;
                }

                G.conditionsOfVictoryAndDefeat.EnemyMove();
            }

            G.UIController.ActualSelect();

            isMove = true;
        }

        if (G.gameLogic.isPlace)
        {
            isMove = false;
            G.gameLogic.isPlace = false;
        }

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
        if (G.gridController.matrix == null)
            return true;
        var data = G.gridController.matrix.GetData();

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
