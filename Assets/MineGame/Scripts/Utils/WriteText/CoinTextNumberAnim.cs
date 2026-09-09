using System.Collections;
using TMPro;
using UnityEngine;
using Febucci.UI;

public class CoinTextNumberAnim : MonoBehaviour
{
    public TextMeshProUGUI text;

    [Header("Popup Settings")]
    public TextMeshProUGUI textPopup;
    public CanvasGroup popup;
    public TextAnimatorPlayer textAnimator;

    [Header("Animation Settings")]
    public float popupDuration = 0.3f;
    public float numberChangeDelay = 0.15f;
    public string popupAppearEffect = "<shake>"; 
    public string popupDisappearEffect = "<fade>"; 

    [Header("Colors")]
    public Color increaseColor = Color.green;
    public Color decreaseColor = Color.red;
    public Color defaultColor = Color.white;

    public int coin = 0;
    private int toCoin = 0;
    private bool isCor = false;

    private void Awake()
    {
        popup.alpha = 0;

        if (textPopup != null)
            textPopup.color = defaultColor;
    }

    public void EditCoin(int num)
    {
        toCoin = num;

        if (!isCor)
            StartCoroutine(popupAnim());
    }

    private IEnumerator popupAnim()
    {
        isCor = true;

        yield return StartCoroutine(ShowPopupWithEffect());

        yield return StartCoroutine(AnimateNumberChange());

        isCor = false;

        yield return StartCoroutine(HidePopupWithEffect());
    }

    private IEnumerator ShowPopupWithEffect()
    {
        popup.alpha = 1;

        Color initialColor = GetColorForDirection();
        textPopup.color = initialColor;

        string initialText = GetInitialPopupText();
        textPopup.text = popupAppearEffect + initialText;

        textAnimator.ShowText(initialText);

        yield return new WaitForSeconds(popupDuration);
    }

    private IEnumerator AnimateNumberChange()
    {
        while (coin != toCoin)
        {
            string popupText = "";
            Color textColor = defaultColor;

            if (toCoin > coin)
            {
                int diff = toCoin - coin;
                popupText = $"+{diff}";
                textColor = increaseColor;
                coin++;
            }
            else
            {
                int diff = coin - toCoin;
                popupText = $"-{diff}";
                textColor = decreaseColor;
                coin--;
            }

            textPopup.color = textColor;
            textPopup.text = popupAppearEffect + popupText;
            text.text = $"{coin}";

            textAnimator.ShowText(popupText);

            yield return new WaitForSeconds(numberChangeDelay);
        }
    }

    private IEnumerator HidePopupWithEffect()
    {
        float elapsed = 0f;
        float startAlpha = popup.alpha;

        while (elapsed < popupDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / popupDuration;
            popup.alpha = Mathf.Lerp(startAlpha, 0f, t);

            Color currentColor = textPopup.color;
            currentColor.a = Mathf.Lerp(1f, 0f, t);
            textPopup.color = currentColor;

            yield return null;
        }

        popup.alpha = 0;

        textPopup.color = defaultColor;
    }

    private string GetInitialPopupText()
    {
        if (toCoin > coin)
            return $"+{toCoin - coin}";
        else if (toCoin < coin)
            return $"-{coin - toCoin}";
        else
            return "0";
    }

    private Color GetColorForDirection()
    {
        if (toCoin > coin)
            return increaseColor;
        else if (toCoin < coin)
            return decreaseColor;
        else
            return defaultColor;
    }

    public void SetPopupColor(Color newColor)
    {
        if (textPopup != null)
            textPopup.color = newColor;
    }

    public IEnumerator LerpPopupColor(Color targetColor, float duration)
    {
        Color startColor = textPopup.color;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            textPopup.color = Color.Lerp(startColor, targetColor, t);
            yield return null;
        }

        textPopup.color = targetColor;
    }

    public void EditCoinWithFlash(int num)
    {
        toCoin = num;

        if (!isCor)
            StartCoroutine(FlashAnim());
    }

    private IEnumerator FlashAnim()
    {
        isCor = true;

        yield return StartCoroutine(ShowPopupWithEffect());

        while (coin != toCoin)
        {
            string popupText = "";
            Color textColor = defaultColor;

            if (toCoin > coin)
            {
                int diff = toCoin - coin;
                popupText = $"+{diff}";
                textColor = increaseColor;
                coin++;
            }
            else
            {
                int diff = coin - toCoin;
                popupText = $"-{diff}";
                textColor = decreaseColor;
                coin--;
            }

            textPopup.color = textColor;
            textPopup.text = popupAppearEffect + popupText;
            text.text = $"{coin}";

            Vector3 originalScale = textPopup.transform.localScale;
            textPopup.transform.localScale = originalScale * 1.2f;

            textAnimator.ShowText(popupText);

            yield return new WaitForSeconds(numberChangeDelay * 0.5f);

            textPopup.transform.localScale = originalScale;

            yield return new WaitForSeconds(numberChangeDelay * 0.5f);
        }

        isCor = false;

        yield return StartCoroutine(HidePopupWithEffect());
    }
}