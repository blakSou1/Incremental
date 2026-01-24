using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class GameMode : MonoBehaviour
{
    public GridBox.Status playerColor = GridBox.Status.Black;

    public TextThrower motionText;

    public GridFuncion gridFuncion;
    public GameLogic gameLogic;

    [NonSerialized] public Text indicatorText = null;

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
        G.run = new();

        motionText.ThrowText(new LocString("", ""), R.normalVoice);

        indicatorText = GameObject.FindGameObjectWithTag("Indicator").GetComponent<Text>();

        G.gridFuncion = gridFuncion;
        G.gameLogic = gameLogic;
        G.modifirePieces = new();

        G.modifirePieces.Init();
        gameLogic.Init();
        gridFuncion.Init();

        G.PlayerController.playerColor = (GridBox.Status)((int)G.gameMode.playerColor * -1);
    }
    public void StartGame()
    {
        G.AudioManager.PlayMusic(R.Audio.tutorial);

        gridFuncion.InitIndexPos();
    }

    public void EndStartAnimation()
    {
        G.PlayerController.isStopped = false;

        G.ai.InitWeight();

        StartCoroutine(SpawnEnemy());
    }

    private IEnumerator SpawnEnemy()
    {
        yield return StartCoroutine(G.PlayerController.MoveAndRotate(G.PlayerController.position1, G.PlayerController.position2));

        G.enemySprite.UpdateSprite();

        yield return new WaitForSeconds(1f);

        yield return StartCoroutine(G.PlayerController.MoveAndRotate(G.PlayerController.position2, G.PlayerController.position1));

        G.modifirePieces.SpawnPiece();

        StartCoroutine(gameLogic.InitStaticPieces());
    }

    public  void IndicatorText(string text)
    {
        if(indicatorText != null)
            indicatorText.text = text;
    }

    public void PlayerInputUpdate()
    {
        if (disableInputForPass || isGameEnd || disableInputForSelect)
            G.inputs.Player.Disable();
        else
            G.inputs.Player.Enable();
    }

    public void RestartGame()
    {
        gridFuncion.ClearAllPieces();

        StartCoroutine(Next());
        IEnumerator Next()
        {
            yield return new WaitForSeconds(0.5f);
            StartCoroutine(gameLogic.InitStaticPieces());
        }
    }

}