using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private List<GeneralButton> buttons;
    private List<LocString> buttonsText = new()
    {
        new LocString("Play", "Играть"),
        new LocString("Exit", "Выход"),
    };

    private void Awake()
    {
        Init();
    }

    private void Start()
    {
        G.LocSystem.CreateLocalizationPanel();
    }

    private void Init()
    {
        buttons = FindObjectsByType<GeneralButton>(FindObjectsInactive.Include, FindObjectsSortMode.None)
        .Where(obj => obj.transform.parent.GetComponent<VerticalLayoutGroup>() != null)
        .OrderBy(b => b.transform.GetSiblingIndex())
        .ToList();

        for (int i = 0; i < buttons.Count; i++)
            buttons[i].transform.GetChild(0).gameObject.AddComponent<UITextSetter>().Init(buttonsText[i]);

        buttons[0].OnClick.AddListener(() => G.SceneLoader.Load("MainGame"));
    }
}
