using System.Collections;

public class WinAndLouse
{
    public void WinPlayer() =>
        G.mainEnterPoint.StartCoroutine(Win());
    public void WinEnemy() =>
        G.mainEnterPoint.StartCoroutine(Loss());

    private IEnumerator Win()
    {
        G.UIController.IndicatorText("WIN");
        G.UIController.motionText.ThrowText(new LocString("You Win!", "Победа!"), R.normalVoice);

        G.enemySprite.animationController.SetAnimation(G.configGame.GetConfigLevel().GetConfigEnemy().enemyWinAnim);
        yield return null;
    }
    private IEnumerator Loss()
    {
        G.UIController.IndicatorText("LOSS");
        G.UIController.motionText.ThrowText(new LocString("Loss!", "Проиграл!"), R.normalVoice);

        G.enemySprite.animationController.SetAnimation(G.configGame.GetConfigLevel().GetConfigEnemy().enemyLouseAnim);
        yield return null;
    }
}
