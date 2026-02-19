using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class Loss : MonoBehaviour
{
    public List<GameObject> enableObject;
    public List<EnableAndMove> enableAndMoveObject;
    public Volume volume;

    public void Start()
    {
        volume.weight = 0;

        G.louse = this;

        foreach(GameObject i in enableObject)
            i.SetActive(false);
        foreach (EnableAndMove i in enableAndMoveObject)
            i.objectMove.SetActive(false);
    }

    public void Louses()
    {
        foreach (GameObject i in enableObject)
            i.SetActive(true);

        foreach (EnableAndMove i in enableAndMoveObject)
        {
            i.objectMove.SetActive(true);
            i.objectMove.transform.DOLocalMoveY(i.endPosY, .2f);
        }

        G.UIController.IndicatorText("LOSS");
        G.UIController.motionText.ThrowText(new LocString("", ""), R.normalVoice);

        G.AudioManager.StopMusic();

        StartCoroutine(ChangeVolumeWeight());
    }
    private IEnumerator ChangeVolumeWeight()
    {
        float startWeight = volume.weight;
        float elapsedTime = 0f;

        while (elapsedTime < .3f)
        {
            volume.weight = Mathf.Lerp(startWeight, 1, elapsedTime / .3f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        volume.weight = 1;

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
