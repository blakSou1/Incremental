using UnityEngine;

[CreateAssetMenu(fileName = "GameHelper", menuName = "Helpers/Game")]
public class GameHelper : ScriptableObject
{
    public void StartGame()
    {
        G.configGame.GetConfigLevel().brain.StartLvl();
    }

    public void RestartGame()
    {
        if (G.mainEnterPoint != null)
            G.mainEnterPoint.StartCoroutine(G.mainEnterPoint.RestartGame());
    }
}