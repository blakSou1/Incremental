using System.Collections;
using UnityEngine;

public class WinAndLoss
{
    public void WinPlayer() =>
        G.mainEnterPoint.StartCoroutine(Win());
    public void WinEnemy() =>
        G.mainEnterPoint.StartCoroutine(Loss());

    private IEnumerator Win()
    {
        G.UIController.IndicatorText("WIN");
        G.UIController.motionText.ThrowText("You Win!", R.normalVoice);

        G.enemySprite.animationController.SetAnimation(G.configGame.GetConfigLevel().enemyConfig.enemyWinAnim);

        G.enemySprite.animationController.endAnimation.AddListener(() => G.PiecePicker.StartCoroutine(G.PiecePicker.StartPicker()));
        G.PiecePicker.isEndPick = false;

        yield return G.PiecePicker.StartCoroutine(PickedEnd());

        G.loss.StartCoroutine(G.loss.Win());
    }

    private IEnumerator PickedEnd()
    {
        while (!G.PiecePicker.isEndPick)
            yield return new WaitForSeconds(.2f);

        yield return new WaitForSeconds(1.2f);
    }

    private IEnumerator Loss()
    {
        G.UIController.IndicatorText("LOSS");
        G.UIController.motionText.ThrowText("Loss!", R.normalVoice);

        G.enemySprite.animationController.SetAnimation(G.configGame.GetConfigLevel().enemyConfig.enemyLouseAnim);
        yield return null;

        G.loss.StartCoroutine(G.loss.Louses());

    }
}
