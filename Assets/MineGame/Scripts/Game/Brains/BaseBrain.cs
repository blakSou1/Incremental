using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using static UnityEditor.SceneView;

public class BaseBrain : MonoBehaviour
{

    public virtual void StartLvl()
    {
    }

    public virtual void Tick()
    {
    }

    public virtual void Resets()
    {
    }

    protected IEnumerator SpawnEnemy()
    {
        G.enemySprite.UpdateSprite();

        yield return new WaitForSeconds(1f);
    }
}

public class BoardBrain : BaseBrain
{
    [NonSerialized]
    protected bool PlaybleStartAnimationSpawnBoard = true;

    public bool isMove = false;

    public override void StartLvl()
    {
        G.AudioManager.PlayMusic(R.Audio.tutorial);

        G.roomMovement.SpawnAndMove(G.configGame.GetConfigLevel().prefabRoom);
        G.roomMovement.OnReachedEnd.AddListener(EndMove);
    }

    private void EndMove()
    {
        PlaybleStartAnimationSpawnBoard = true;
        G.mainEnterPoint.gridController.NewMatrix();
        G.eventManager.host.StartCoroutine(StartAnimationSpawnGrid(G.configGame.MatrixModel.matrixField.size,
            G.mainEnterPoint.gridController.matrix.GetData(), G.roomMovement.parentGrid));
    }

    public override void Tick()
    {
        if (PlaybleStartAnimationSpawnBoard)
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
            PlaybleStartAnimationSpawnBoard = false;
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

        G.eventManager.host.StartCoroutine(Next());
        static IEnumerator Next()
        {
            yield return new WaitForSeconds(UnityEngine.Random.Range(0.65f, 1.4f));
            G.ai.Execute(G.mainEnterPoint.gameLogic.ActualColor);
        }

        G.UIController.SelectEnemy();
    }

    protected virtual IEnumerator StartAnimationSpawnGrid(int index, GridBox[,] data, Transform parent)
    {
        yield return G.eventManager.host.StartCoroutine(SpawnEnemy());
        yield return new WaitForSeconds(.4f);

        yield return G.PlayerController.MoveTo(G.roomMovement.GridCameraPosition, 1.5f);

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
                data[i, j].transform.localPosition = new(G.boardVisualConfig.cellSpacing.x * i - offsetX, 0, G.boardVisualConfig.cellSpacing.y * j - offsetY);

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

        G.eventManager.host.StartCoroutine(EndStartAnimation());
    }

    public virtual IEnumerator EndStartAnimation()
    {
        yield return G.eventManager.host.StartCoroutine(G.mainEnterPoint.gameLogic.InitStaticPieces());

        yield return new WaitForSeconds(.3f);

        G.mainEnterPoint.pieceController.StartInitModPiece();

        yield return new WaitForSeconds(.3f);

        G.mainEnterPoint.gridController.StartInitModGrid();

        PlaybleStartAnimationSpawnBoard = false;

        G.mainEnterPoint.gridController.CreateIndisObject(G.mainEnterPoint.gameLogic.ActualColor, G.mainEnterPoint.gameLogic.actualPieceInstance);
    }

    protected virtual void EndTick()
    {

    }

}