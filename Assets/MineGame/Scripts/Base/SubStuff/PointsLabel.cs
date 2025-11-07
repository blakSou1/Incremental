using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Text))]
public class PointsLabel : MonoBehaviour
{
    private TMP_Text label;
    
    void Awake()
    {
        label = GetComponent<TMP_Text>();
    }

    void Update()
    {
        label.text = new LocString("Points: ", "Очки: ") + ((int)G.GameState.Points).ToString();
    }
}
