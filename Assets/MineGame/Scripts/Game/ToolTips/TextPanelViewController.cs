using TMPro;
using UnityEngine;

public class TextPanelViewController : MonoBehaviour
{
    public TMP_Text Text;
    public UnityEngine.UI.Image Image;

    public float Padding = 10f;

    public bool Center = false;
    private Vector2 _initialPosition;
    private Vector2 _initialSize;


    private void Start() {
        _initialPosition = (transform as RectTransform).anchoredPosition;
        _initialSize = Image.rectTransform.sizeDelta - Vector2.right * Padding;
    }

    private void FixedUpdate() {
        Image.rectTransform.sizeDelta = new Vector2(Text.renderedWidth + Padding, 30);

        if (Center) {
            var sizeDiff = Image.rectTransform.sizeDelta - _initialSize;
            (transform as RectTransform).anchoredPosition = _initialPosition - new Vector2(sizeDiff.x / 2f, 0);
        }
    }
}
