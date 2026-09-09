using UnityEngine;

[CreateAssetMenu(fileName = "UIHelper", menuName = "Helpers/UI")]
public class UIHelper : ScriptableObject
{
    public void UpdateFAQPanel()
    {
        G.faqPanel.UpdatePanelFAQ();
    }

    public void ShowSettings()
    {
        G.pausePanel.UpdatePanels();
    }
}