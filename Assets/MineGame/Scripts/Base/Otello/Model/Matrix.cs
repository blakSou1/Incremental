using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;
using Index = System.Tuple<int, int>;

[Serializable]
public class Matrix
{
    private GridBox[,] data;

    private Transform parent;

    public Matrix(int index)
    {
        if (parent != null)
            GameObject.Destroy(parent.gameObject);
        parent = new GameObject("GridBoxParent").transform;

        data = new GridBox[index, index];

        G.gameMode.StartCoroutine(CreateMatrix(index));
    }

    private IEnumerator CreateMatrix(int index)
    {
        float offsetX = (index - 1) * G.gameMode.gridFuncion.indentGrid.x / 2;
        float offsetY = (index - 1) * G.gameMode.gridFuncion.indentGrid.y / 2;

        for (int i = 0; i < index; i++)
        {
            for (int j = 0; j < index; j++)
            {
                data[i, j] = GameObject.Instantiate(G.gameMode.gridFuncion.prefabGridBox, parent);
                data[i, j].transform.position = new(G.gameMode.gridFuncion.indentGrid.x * i - offsetX, G.gameMode.gridFuncion.indentGrid.y * j - offsetY);

                data[i, j].SetIndex(i+1, j+1);

                Vector3 scale = data[i, j].transform.localScale;

                data[i, j].transform.localScale = Vector3.zero;
                Tween tween = data[i, j].transform.DOScale(scale, 0.3f).SetEase(Ease.OutBounce);

                yield return new WaitForSeconds(.01f);
            }
        }

        G.gameMode.EndStartAnimation();
    }

    public GridBox[,] GetData()
    {
        return data;
    }

    public GridBox GetGrid(Index index)
    {
        if (data == null || index == null) return null;

        return data[index.Item1 - 1, index.Item2 - 1];
    }
}
