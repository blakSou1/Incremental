using System;
using System.Collections;
using TMPro;
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

    [Space]
    public TextMeshProUGUI PlayerHpText;
    public Image PlayerHpImage;

    [Space]
    public Image dropEffect;
    public float dropSpeed = 0.5f;
    private float dropEffectPercentage = 1;

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

        UpdatePlayerHp();
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

    public void UpdatePlayerHp()
    {
        StartCoroutine(UpdateHp());
    }

    private IEnumerator UpdateHp()
    {
        PlayerHpText.text = $"{G.run.health}";

        float healthPercentage = Mathf.Min(Mathf.Max(0, G.run.health / G.run.maxHealth), 1);

        PlayerHpImage.fillAmount = healthPercentage;

        while(dropEffectPercentage > healthPercentage)
        {
            dropEffectPercentage -= Time.deltaTime * dropSpeed;
            dropEffect.fillAmount = dropEffectPercentage;

            yield return null;
        }

        dropEffectPercentage = healthPercentage;
    }
}
