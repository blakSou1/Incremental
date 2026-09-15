using UnityEngine;

[CreateAssetMenu(fileName = "GameHelper", menuName = "Helpers/Game")]
public class GameHelper : ScriptableObject
{
    public void StartGame()
    {
        G.configGame.GetConfigLevel().brain.StartLvl();
    }
}