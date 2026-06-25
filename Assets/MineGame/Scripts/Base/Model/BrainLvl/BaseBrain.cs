using System;
using System.Collections;
using UnityEngine;

public class BaseBrain : MonoBehaviour
{
    [NonSerialized]
    protected bool startIsEnd = false;

    public virtual void StartLvl()
    {
        startIsEnd = true;
    }

    public virtual void Tick()
    {

    }

    protected IEnumerator SpawnEnemy()
    {
        yield return G.mainEnterPoint.StartCoroutine(G.PlayerController.MoveAndRotate(G.PlayerController.position1, G.PlayerController.position2));

        G.enemySprite.UpdateSprite();

        yield return new WaitForSeconds(1f);

        yield return G.mainEnterPoint.StartCoroutine(G.PlayerController.MoveAndRotate(G.PlayerController.position2, G.PlayerController.position1));
    }

    protected IEnumerator VisibalTextActualLvl()
    {
        if (G.run.indexLvl == G.configGame.indexWinLvl)
        {
            G.SceneLoader.Load("Win");

            yield break;
        }

        G.UIController.textActualLvl.text = G.configGame.GetConfigLevel().preview;

        yield return G.mainEnterPoint.StartCoroutine(G.UIController.FadeCanvasGroup(G.UIController.GroupTextActualLvl, 1f, 1.2f));

        yield return new WaitForSeconds(.8f);

        yield return G.mainEnterPoint.StartCoroutine(G.UIController.FadeCanvasGroup(G.UIController.GroupTextActualLvl, 0f));

    }
}
