using System.Collections;

public class WinAndLouse
{
    public IEnumerator Win()
    {
        G.gameMode.IndicatorText("WIN");

        G.enemySprite.animationController.SetAnimation(G.configGame.GetConfigLevel().GetConfigEnemy().enemyWinAnim);
        yield return null;
    }
    public IEnumerator Loss()
    {
        G.gameMode.IndicatorText("LOSS");

        G.enemySprite.animationController.SetAnimation(G.configGame.GetConfigLevel().GetConfigEnemy().enemyLouseAnim);
        yield return null;
    }



}
