using System.Collections;
using UnityEngine;

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

        G.enemySprite.animationController.SetAnimation(G.configGame.GetConfigLevel().enemyConfig.enemyWinAnim);

        G.enemySprite.animationController.endAnimation.AddListener(() => G.PiecePicker.StartCoroutine(G.PiecePicker.StartPicker()));
        G.PiecePicker.isEndPick = false;

        yield return G.PiecePicker.StartCoroutine(PickedEnd());

        G.louse.StartCoroutine(G.louse.Win());
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
        G.UIController.motionText.ThrowText(new LocString("Loss!", "Проиграл!"), R.normalVoice);

        G.enemySprite.animationController.SetAnimation(G.configGame.GetConfigLevel().enemyConfig.enemyLouseAnim);
        yield return null;

        G.louse.StartCoroutine(G.louse.Louses());

    }
}
