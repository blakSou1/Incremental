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
    }

    public GridBox[,] GetData()
    {
        return data;
    }

    public Transform GetParent()
    {
        return parent;
    }

    public GridBox GetGrid(Index index)
    {
        if (data == null || index == null) return null;

        return data[index.Item1, index.Item2];
    }
}
