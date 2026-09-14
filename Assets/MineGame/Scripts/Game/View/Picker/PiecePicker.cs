using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecePicker : MonoBehaviour
{
    public GameObject isButtonPicker;
    public CanvasGroup groupTextPicker;

    public float minX, maxX;

    private void Start()
    {
        G.PiecePicker = this;
    }

    public bool isEndPick = false;
    public IEnumerator StartPicker()
    {
        yield return StartCoroutine(G.enemySprite.DisableSprite());

        List<string> listid = G.configGame.GetConfigLevel().GetPickablePiece();

        if(listid.Count == 0)
        {
            isEndPick = true;
            StartCoroutine(G.UIController.FadeCanvasGroup(groupTextPicker, 0));

            yield break;
        }

        List<InteractiveObject> piece = new();

        foreach (var i in listid)
            piece.Add(G.pieceFactory.AddPiece(i));
        foreach (var i in piece)
            i.moveable.isStop = true;

        yield return null;
        
        DistributeHorizontally(piece);
    }

    public void DistributeHorizontally(List<InteractiveObject> objects)
    {
        if (objects == null || objects.Count == 0) return;

        if (objects.Count == 1)
        {
            float centerX = (minX + maxX) / 2f;

            objects[0].transform.localPosition = new Vector3(centerX, 0, 0);

            SpawnUiButton(objects[0]);

            StartCoroutine(G.UIController.FadeCanvasGroup(groupTextPicker, 1));

            return;
        }

        float center = (minX + maxX) / 2f;
        float totalWidth = maxX - minX;
        float step = totalWidth / (objects.Count - 1);
        float startX = center - totalWidth / 2f;

        for (int i = 0; i < objects.Count; i++)
        {
            if (objects[i] == null) continue;

            float x = startX + i * step;

            Vector3 localPos = Vector3.zero;
            localPos.x = x;

            objects[i].transform.localPosition = localPos;

            SpawnUiButton(objects[i]);
        }

        StartCoroutine(G.UIController.FadeCanvasGroup(groupTextPicker, 1));
    }

    private void SpawnUiButton(InteractiveObject objectP)
    {
        GameObject ob = Instantiate(isButtonPicker);
        ob.transform.SetParent(transform, false);

        objectP.GetComponentInChildren<ReactToPointer>().TooltipMessage = (objectP.state.model as PieceBase).Description;

        ob.transform.localPosition = new(objectP.transform.localPosition.x, 0, 0);

        GeneralButton b = ob.GetComponentInChildren<GeneralButton>();

        objectP.transform.localPosition = new Vector3(0, 0, 0);
        objectP.transform.SetParent(b.transform.GetChild(0), false);

        b.OnClick.AddListener(() => TaskButton(b));
    }

    private void TaskButton(GeneralButton b)
    {
        G.AudioManager.PlaySound(R.Audio.part, .6f);

        InteractiveObject objectP = b.transform.GetChild(0).GetChild(0).GetComponent<InteractiveObject>();

        G.run.deck.Add(objectP.state.model.id);

        List<Transform> toDestroy = new();
        for (int i = 0; i < transform.childCount; i++)
            if (transform.GetChild(i).gameObject != objectP.gameObject)
                toDestroy.Add(transform.GetChild(i));

        foreach (var t in toDestroy)
            Destroy(t.gameObject);

        objectP.moveable.targetPosition = new(-20, -2, 0);
        objectP.moveable.isStop = false;

        isEndPick = true;
        StartCoroutine(G.UIController.FadeCanvasGroup(groupTextPicker, 0));
    }

}
