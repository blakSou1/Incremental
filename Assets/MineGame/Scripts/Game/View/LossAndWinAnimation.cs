using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LossAndWinAnimation : MonoBehaviour
{
    public List<GameObject> enableObject;
    public List<EnableAndMove> enableAndMoveObject;

    public void Start()
    {
        G.eventManager.PlayerWin.AddListener(Win);
        G.eventManager.PlayerLose.AddListener(Losses);

        G.eventManager.VolumeWeugth.Invoke(0);

        foreach(GameObject i in enableObject)
            i.SetActive(false);
        foreach (EnableAndMove i in enableAndMoveObject)
            i.objectMove.SetActive(false);
    }

    private void Win()
    {
        G.UIController.IndicatorText("WIN");
        G.UIController.motionText._textAnimator.ShowText("");

        G.AudioManager.StopMusic();

        G.run.currentLevel++;

        var currentScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
        G.SceneLoader.Load(currentScene.name);
    }

    private void Losses()
    {
        foreach (GameObject i in enableObject)
            i.SetActive(true);

        foreach (EnableAndMove i in enableAndMoveObject)
        {
            i.objectMove.SetActive(true);
            i.objectMove.transform.DOLocalMoveY(i.endPosY, .2f);
        }

        G.UIController.IndicatorText("LOSS");
        G.UIController.motionText._textAnimator.ShowText("");

        G.AudioManager.StopMusic();

        StartCoroutine(ChangeVolumeWeight());
    }
    
    private IEnumerator ChangeVolumeWeight()
    {
        float startWeight = 0;
        float elapsedTime = 0f;

        while (elapsedTime < .3f)
        {
            G.eventManager.VolumeWeugth.Invoke(Mathf.Lerp(startWeight, 1, elapsedTime / .3f));
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        G.eventManager.VolumeWeugth.Invoke(1);

        yield return new WaitForSeconds(.3f);

        yield return StartCoroutine(G.enemySprite.DisableSprite());

        G.SceneLoader.Load("MainMenu");
    }

}

[Serializable]
public class EnableAndMove
{
    public GameObject objectMove;
    public float endPosY;
}
