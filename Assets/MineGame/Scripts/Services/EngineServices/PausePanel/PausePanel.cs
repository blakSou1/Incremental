using UnityEngine;
using UnityEngine.UI;

public class PausePanel : MonoBehaviour, IService
{
    public UIPanelScaler panel;

    public bool inMenu = false;

    public void Init()
    {
        GameObject can = new("MainMenuCanvas");
        DontDestroyOnLoad(can);

        Canvas c = can.AddComponent<Canvas>();

        CanvasScaler cs = can.AddComponent<CanvasScaler>();
        cs.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        cs.referenceResolution = new Vector2(1920, 1080);
        cs.matchWidthOrHeight = 1;

        can.AddComponent<GraphicRaycaster>();
        c.worldCamera = Camera.main;
        c.renderMode = RenderMode.ScreenSpaceOverlay;
        c.sortingOrder = 500;

        GameObject temp = Resources.Load<GameObject>("Services/" + "PauseMenu");
        temp.SetActive(false);
        GameObject g = Instantiate(temp, can.transform);
        panel = g.GetComponent<UIPanelScaler>();

        G.inputs.Player.Esc.performed += i => UpdatePanel();
    }

    void OnDestroy()
    {
        G.inputs.Player.Esc.performed -= i => UpdatePanel();
    }

    private void UpdatePanel()
    {
        if (!panel.inAnim)
        {
            UpdatePanels();
        }
    }
    public void UpdatePanels()
    {
        if (panel.gameObject.activeSelf)
        {
            panel.Close();
            inMenu = false;
        }
        else
        {
            panel.Open();
            inMenu = true;
        }
    }
}
