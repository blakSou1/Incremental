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
    }

    private void InitComponents()
    {
        indicatorText = GameObject.FindGameObjectWithTag("Indicator").GetComponent<Text>();

        G.gridFuncion = gridFuncion;
        G.gameLogic = gameLogic;

        gameLogic.Init();
        gridFuncion.Init();
    }
    public void StartGame()
    {
        gridFuncion.InitIndexPos();
    }

    public void EndStartAnimation()
    {
        G.PlayerController.isStopped = false;

        gameLogic.InitStaticPieces();
        gridFuncion.ShowPossibleLocation(G.PlayerController.playerColor);
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
        G.PlayerController.playerColor = playerColor;

        StartCoroutine(Next());
        IEnumerator Next()
        {
            yield return new WaitForSeconds(0.5f);
            gameLogic.InitStaticPieces();
        }
    }

}