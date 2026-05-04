using TMPro;
using UnityEngine;

public class PointerViewController : MonoBehaviour
{
    private ControlSystem Control;

    [SerializeField] private Vector3 Offset;
    private TMP_Text TooltipText;
    private GameObject TooltipDisplay;

    private void Start() {
        Control = FindFirstObjectByType<ControlSystem>();
        TooltipText = gameObject.GetComponentInChildren<TMP_Text>();
        TooltipDisplay = transform.GetChild(0).gameObject;

        Control.PointerTooltipUpdated += (msg) => UpdateText(msg);
    }

    public void UpdatePosition(Vector3 Position)
    {
        transform.position = Position + Offset;
    }
    private void UpdateText(string msg) {
        if (!string.IsNullOrEmpty(msg)) {
            TooltipDisplay.SetActive(true);
            TooltipText.text = msg;
        }
        else {
            TooltipDisplay.SetActive(false);
        }
    }
}
