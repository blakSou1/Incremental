using DG.Tweening;
using System.Collections;
using UnityEngine;

public class MainEnterPoint : MonoBehaviour
{
    public GridBox.Status playerColor = GridBox.Status.Black;

    [HideInInspector] public bool disableInputForPass = false;
    [HideInInspector] public bool isGameEnd = false;
    [HideInInspector] public bool disableInputForSelect = false;

    private void Start()
    {
        InitComponents();
        StartGame();
    }

    private void InitComponents()
    {
        G.pieceController = new();
        G.gridController = new();
        G.modifirePieces = new();
        G.conditionsOfVictoryAndDefeat = new();

        G.pieceController.Init();
        G.gridController.Init();
        G.modifirePieces.Init();
        G.conditionsOfVictoryAndDefeat.Init();

        G.PlayerController.playerColor = (GridBox.Status)((int)playerColor * -1);
    }
    
    public void StartGame()
    {
        G.AudioManager.PlayMusic(R.Audio.tutorial);

        G.gridController.NewMatrix();
        StartCoroutine(StartAnimationSpawnGrid(G.configGridFunction.item1, G.gridController.matrix.GetData(), G.gridController.matrix.GetParent()));
    }

    private IEnumerator StartAnimationSpawnGrid(int index, GridBox[,] data, Transform parent)
    {
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

        EndStartAnimation();
    }

    public void EndStartAnimation()
    {
        G.PlayerController.isStopped = false;

        StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy()
    {
        yield return StartCoroutine(G.PlayerController.MoveAndRotate(G.PlayerController.position1, G.PlayerController.position2));

        G.enemySprite.UpdateSprite();

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(G.PlayerController.MoveAndRotate(G.PlayerController.position2, G.PlayerController.position1));

        yield return StartCoroutine(G.gameLogic.InitStaticPieces());

        yield return new WaitForSeconds(.3f);

        G.pieceController.StartInitModPiece();

        yield return new WaitForSeconds(.3f);

        G.gridController.StartInitModGrid();
    }


    //SoProgram
    public void PlayerInputUpdate()
    {
        if (disableInputForPass || isGameEnd || disableInputForSelect)
            G.inputs.Player.Disable();
        else
            G.inputs.Player.Enable();
    }

    public IEnumerator RestartGame()
    {
        G.gridController.ClearAllPieces();

        yield return new WaitForSeconds(0.5f);
        StartCoroutine(G.gameLogic.InitStaticPieces());
    }

}