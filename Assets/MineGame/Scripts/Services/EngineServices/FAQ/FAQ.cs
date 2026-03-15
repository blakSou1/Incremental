using UnityEngine;
using UnityEngine.UI;

public class FAQ : MonoBehaviour, IService
{
    public UIPanelScaler panel;
    public bool inFAQ = false;

    public void Init()
    {
        GameObject can = new("FAQCanvas");
        DontDestroyOnLoad(can);

        Canvas c = can.AddComponent<Canvas>();

        CanvasScaler cs = can.AddComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        cs.matchWidthOrHeight = 1;

        can.AddComponent<GraphicRaycaster>();
        c.worldCamera = Camera.main;
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        c.sortingOrder = 550;

        GameObject temp = Resources.Load<GameObject>("Services/" + "FAQ");
        temp.SetActive(false);
        GameObject g = Instantiate(temp, can.transform);
        panel = g.GetComponent<UIPanelScaler>();
    }
    public void UpdatePanelFAQ()
    {
        if (!panel.inAnim)
            UpdatePanels();
    }
    private void UpdatePanels()
    {
        if (panel.gameObject.activeSelf)
        {
            panel.Close();
            inFAQ = false;
        }
        else
        {
            panel.Open();
            inFAQ = true;
        }
    }

}
