using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class BaseBrain : MonoBehaviour
{
    [NonSerialized]
    protected bool startIsEnd = false;

    public virtual void StartLvl()
    {
        startIsEnd = true;
    }

    public virtual void Tick()
    {
    }

    protected virtual IEnumerator StartAnimationSpawnGrid(int index, GridBox[,] data, Transform parent)
    {
        yield return G.mainEnterPoint.StartCoroutine(VisibalTextActualLvl());

        float offsetX = (index - 1) * G.boardVisualConfig.cellSpacing.x / 2;
        float offsetY = (index - 1) * G.boardVisualConfig.cellSpacing.y / 2;

        System.Random random = new();
        bool isAudi = false;

        for (int i = 0; i < index; i++)
        {
            for (int j = 0; j < index; j++)
            {
                int randomIndex = random.Next(1, 4);

                data[i, j] = GameObject.Instantiate(G.boardVisualConfig.cellPrefab, parent);
                data[i, j].transform.position = new(G.boardVisualConfig.cellSpacing.x * i - offsetX, G.boardVisualConfig.cellSpacing.y * j - offsetY);

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

    public virtual IEnumerator EndStartAnimation()
    {
        G.PlayerController.isStopped = false;

        yield return G.mainEnterPoint.StartCoroutine(SpawnEnemy());


        yield return G.mainEnterPoint.StartCoroutine(G.mainEnterPoint.gameLogic.InitStaticPieces());

        yield return new WaitForSeconds(.3f);

        G.mainEnterPoint.pieceController.StartInitModPiece();

        yield return new WaitForSeconds(.3f);

        G.mainEnterPoint.gridController.StartInitModGrid();

        startIsEnd = true;
    }

    protected IEnumerator SpawnEnemy()
    {
        yield return G.mainEnterPoint.StartCoroutine(G.PlayerController.MoveAndRotate(G.PlayerController.cameraPositionordinary, G.PlayerController.cameraPositionVisibalEnemy));

        G.enemySprite.UpdateSprite();

        yield return new WaitForSeconds(1f);

        yield return G.mainEnterPoint.StartCoroutine(G.PlayerController.MoveAndRotate(G.PlayerController.cameraPositionVisibalEnemy, G.PlayerController.cameraPositionordinary));
    }

    protected IEnumerator VisibalTextActualLvl(string text = null)
    {
        if (G.run.currentLevel == G.configGame.indexWinLvl)
        {
            G.SceneLoader.Load("Win");

            yield break;
        }

        if(text == null)
            G.UIController.textActualLvl.text = G.configGame.GetConfigLevel().preview.ToString();
        else
            G.UIController.textActualLvl.text = text;

        yield return G.mainEnterPoint.StartCoroutine(G.UIController.FadeCanvasGroup(G.UIController.GroupTextActualLvl, 1f, 1.2f));

        yield return new WaitForSeconds(.8f);

        yield return G.mainEnterPoint.StartCoroutine(G.UIController.FadeCanvasGroup(G.UIController.GroupTextActualLvl, 0f));

    }
}

public class BoardBrain : BaseBrain
{
    public bool isMove = false;

    public override void Tick()
    {
        if (!startIsEnd)
            return;

        if (!isMove)
            StartMove();

        EndTick();
    }

    protected virtual void StartMove()
    {
        int indicCount = 0;
        if (G.mainEnterPoint.gameLogic.actualPieceInstance != null)
            indicCount = G.mainEnterPoint.gridController.CreateIndisObject(
                G.mainEnterPoint.gameLogic.ActualColor,
                G.mainEnterPoint.gameLogic.actualPieceInstance);

        int whatIsMove = G.mainEnterPoint.conditionsOfVictoryAndDefeat.ShowPossibleLocation(indicCount);
        if (whatIsMove == 1)
        {
            startIsEnd = false;
            return;
        }
        else if(whatIsMove == 2)
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

    protected virtual void PlayerMove()
    {
        isMove = true;

        G.inputs.Player.Enable();

        G.mainEnterPoint.gridController.EnableAndDisableIndc(true);

        G.UIController.motionText.ThrowText("Your move!", R.normalVoice);

        G.UIController.SelectPlayer();
    }

    protected virtual void EnemyMove()
    {
        isMove = true;

        G.inputs.Player.Disable();

        G.mainEnterPoint.gridController.EnableAndDisableIndc(false);

        G.UIController.motionText.ThrowText("The opponent's move!", R.normalVoice);

        G.mainEnterPoint.StartCoroutine(Next());
        static IEnumerator Next()
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.65f, 1.4f));
            G.ai.Execute(G.mainEnterPoint.gameLogic.ActualColor);
        }

        G.UIController.SelectEnemy();
    }

    protected virtual void EndTick()
    {

    }

}