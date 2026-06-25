using System;
using System.Collections.Generic;

public class MatrixRandomNode : MatrixNode
{
    public List<Param> MatrixList;

    public override MatrixModel GetMatrix()
    {
        return MatrixList[UnityEngine.Random.Range(0, MatrixList.Count)].matrixModel;
    }

}

[Serializable]
public class Param
{
    public MatrixModel matrixModel;
    //probability
}