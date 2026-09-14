using System;
using System.Collections.Generic;

public class MatrixRandomNode : MatrixNode
{
    public List<Param> MatrixList;

    public override Param GetMatrix()
    {
        return MatrixList[UnityEngine.Random.Range(0, MatrixList.Count)];
    }

}

[Serializable]
public class Param
{
    public MatrixModel matrixModel;
    //probability
}