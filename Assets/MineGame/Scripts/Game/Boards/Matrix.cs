using System;
using UnityEngine;
using Index = System.Tuple<int, int>;

[Serializable]
public class Matrix
{
    private GridBox[,] data;

    public Matrix(MatrixModel matrixModel)
    {
        data = new GridBox[matrixModel.matrixField.size, matrixModel.matrixField.size];
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
