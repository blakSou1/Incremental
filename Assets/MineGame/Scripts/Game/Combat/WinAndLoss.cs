
public class WinAndLoss
{
    public void Init()
    {
        G.eventManager.PlayerWin.AddListener(Win);
        G.eventManager.PlayerLose.AddListener(Loss);
    }

    private void Win()
    {
        G.UIController.IndicatorText("WIN");
        G.UIController.motionText.ThrowText("You Win!", R.normalVoice);

        G.enemySprite.animationController.Play(G.configGame.GetConfigLevel().enemyConfig.enemyWinAnim);
    }

    private void Loss()
    {
        G.UIController.IndicatorText("LOSS");
        G.UIController.motionText.ThrowText("Loss!", R.normalVoice);

        G.enemySprite.animationController.Play(G.configGame.GetConfigLevel().enemyConfig.enemyLouseAnim);
    }
}
