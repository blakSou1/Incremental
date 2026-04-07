using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TMP_Text))]
[RequireComponent(typeof(RectTransform))]
public class UITextSetter : MonoBehaviour
{
    public LocString text;
    private TMP_Text _tmp;

    public void Start()
    {
        Init(text);
    }

    public void Init(LocString s)
    {
        text = s;
        _tmp = GetComponent<TMP_Text>();
        UpdateUI();
    }
    public void UpdateUI()
    {
        _tmp.SetText(text.ToString());
        RectTransform rectTransform = GetComponent<RectTransform>();
        LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);
    }
}
