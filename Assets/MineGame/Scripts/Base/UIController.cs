using System;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public TextThrower coinTextPlayer;
    public TextThrower coinTextEnemy;

    public GameObject playerSelect;
    public GameObject EnemySelect;

    public TextThrower motionText;

    [NonSerialized] public Text indicatorText = null;

    public void Awake()
    {
        G.UIController = this;
    }

    public void Start()
    {
        indicatorText = GameObject.FindGameObjectWithTag("Indicator").GetComponent<Text>();

        motionText._textAnimator.ShowText("");

        playerSelect.SetActive(false);
        EnemySelect.SetActive(false);
    }

    public void IndicatorText(string text)
    {
        if (indicatorText != null)
            indicatorText.text = text;
    }

    public void UpdateCountPlayers()
    {
        string bText;
        string wText;

        if (G.gridFuncion.blackPieces.Count < 10)
            bText = "0" + G.gridFuncion.blackPieces.Count.ToString();
        else
            bText = G.gridFuncion.blackPieces.Count.ToString();

        if (G.gridFuncion.whitePieces.Count < 10)
            wText = "0" + G.gridFuncion.whitePieces.Count.ToString();
        else
            wText = G.gridFuncion.whitePieces.Count.ToString();

        coinTextPlayer.ThrowText(new LocString(bText, bText), R.normalVoice);
        coinTextEnemy.ThrowText(new LocString(wText, wText), R.normalVoice);
    }

    public void ActualSelect()
    {
        if (G.PlayerController.playerColor == G.gameMode.playerColor)
        {
            playerSelect.SetActive(true);
            EnemySelect.SetActive(false);
        }
        else
        {
            EnemySelect.SetActive(true);
            playerSelect.SetActive(false);
        }
    }
}
