using TMPro;
using UnityEngine;

public class TempWinStuff : MonoBehaviour
{

    private TMP_Text label;
    void Awake()
    {
        label = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (G.GameState.Points < 5000)
        {
            label.text = "Чтобы победить, накопи 5000 очков!";
        }
        else
        {
            label.text = "!!!!!!!!ПОБЕДАА!!!!!!!!";
        }
        
    }
}
