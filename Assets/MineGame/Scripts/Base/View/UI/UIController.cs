using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIController : MonoBehaviour
{
    public CoinTextNumderAnim coinPlayer;
    public CoinTextNumderAnim coinEnemy;

    public GameObject playerSelect;
    public GameObject EnemySelect;

    public GameObject ButtonIsPassActiv;
    public GameObject ButtonIsPassDiactive;

    public TextThrower motionText;

    [NonSerialized] public Text indicatorText = null;

    [Space]
    public TextMeshProUGUI PlayerHpText;
    public Image PlayerHpImage;

    [Space]
    public Image dropEffect;
    public float dropSpeed = 0.5f;
    private float dropEffectPercentage = 1;

    [Space]
    public TextMeshProUGUI textActualLvl;
    public CanvasGroup GroupTextActualLvl;

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
        coinPlayer.EditCoin(G.gridController.blackPieces.Count);
        coinEnemy.EditCoin(G.gridController.whitePieces.Count);
    }

    public void ActualSelect()
    {
        if (G.PlayerController.playerColor == G.mainEnterPoint.playerColor)
        {
            playerSelect.SetActive(true);
            ButtonIsPassActiv.SetActive(true);
            ButtonIsPassDiactive.SetActive(false);
            EnemySelect.SetActive(false);
        }
        else
        {
            EnemySelect.SetActive(true);
            ButtonIsPassActiv.SetActive(false);
            ButtonIsPassDiactive.SetActive(true);
            playerSelect.SetActive(false);
        }
    }

    public void UpdatePlayerHp()
    {
        StartCoroutine(UpdateHp());
    }

    public IEnumerator FadeCanvasGroup(CanvasGroup group, float targetAlpha, float duration = .4f)
    {
        if (group == null) yield break;

        float startAlpha = group.alpha;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            if (group == null)
                break;

            elapsedTime += Time.deltaTime;
            float t = Mathf.Clamp01(elapsedTime / duration);
            group.alpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            yield return null;
        }

        group.alpha = targetAlpha;
    }

    private IEnumerator UpdateHp()
    {
        PlayerHpText.text = $"{G.run.maxHealth - G.run.Damage}";

        float healthPercentage = Mathf.Min(Mathf.Max(0, (G.run.maxHealth - G.run.Damage) / G.run.maxHealth), 1);

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
