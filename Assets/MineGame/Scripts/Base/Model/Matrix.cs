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

        System.Random random = new System.Random();
        bool isAudi = false;

        for (int i = 0; i < index; i++)
        {
            for (int j = 0; j < index; j++)
            {
                int randomIndex = random.Next(1, 4);

                data[i, j] = GameObject.Instantiate(G.gameMode.gridFuncion.prefabGridBox, parent);
                data[i, j].transform.position = new(G.gameMode.gridFuncion.indentGrid.x * i - offsetX, G.gameMode.gridFuncion.indentGrid.y * j - offsetY);

                data[i, j].SetIndex(i, j);

                Vector3 scale = data[i, j].transform.localScale;

                data[i, j].transform.localScale = Vector3.zero;
                Tween tween = data[i, j].transform.DOScale(scale, 0.3f).SetEase(Ease.OutBounce);

                yield return new WaitForSeconds(.035f);

                if (isAudi)
                {
                    isAudi = false;
                    continue;
                }

                switch (randomIndex)
                {
                    case 1:
                        G.AudioManager.PlaySound(R.Audio.pop1, -.05f);
                        break;
                    case 2:
                        G.AudioManager.PlaySound(R.Audio.pop2, -.07f);
                        break;
                    case 3:
                        G.AudioManager.PlaySound(R.Audio.pop3, 0);
                        break;
                }

                isAudi = true;
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

        return data[index.Item1, index.Item2];
    }
}
