using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiecePicker : MonoBehaviour
{
    public GameObject isButtonPicker;

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

        List<InteractiveObject> piece = new();

        foreach (var i in listid)
            piece.Add(G.chooice.AddPiece(i));
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
    }

    private void SpawnUiButton(InteractiveObject objectP)
    {
        GameObject ob = Instantiate(isButtonPicker);
        ob.transform.SetParent(transform, false);

        ob.transform.localPosition = new(objectP.transform.localPosition.x, 0, 0);

        GeneralButton b = ob.GetComponentInChildren<GeneralButton>();

        objectP.transform.localPosition = new Vector3(0, 0, 0);
        objectP.transform.SetParent(b.transform.GetChild(0), false);

        b.OnClick.AddListener(() => TaskButton(b));
    }

    private void TaskButton(GeneralButton b)
    {
        InteractiveObject objectP = b.transform.GetChild(0).GetChild(0).GetComponent<InteractiveObject>();

        G.run.pieceBag.Add(objectP.state.model.id);

        for (int i = 0; i < transform.childCount; i++)
            Destroy(transform.GetChild(i).gameObject);

        isEndPick = true;
    }

}
