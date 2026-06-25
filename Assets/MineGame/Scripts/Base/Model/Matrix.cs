using System;
using UnityEngine;
using Index = System.Tuple<int, int>;

[Serializable]
public class Matrix
{
    private GridBox[,] data;

    private Transform parent;

    public Matrix(MatrixModel matrixModel)
    {
        if (parent != null)
            GameObject.Destroy(parent.gameObject);
        parent = new GameObject("GridBoxParent").transform;

        data = new GridBox[matrixModel.matrixField.size, matrixModel.matrixField.size];
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
