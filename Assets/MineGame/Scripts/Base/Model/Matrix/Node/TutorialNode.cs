using System;
using System.Collections.Generic;

public class TutorialNode : MatrixNode
{
    public List<ParamT> MatrixList;
    [NonSerialized] public int index = 0;

    public override MatrixModel GetMatrix()
    {
        return MatrixList[index].matrixModel;
    }

    public void NextIndex()
    {
        if(index < MatrixList.Count)
            index++;
    }

}

[Serializable]
public class ParamT
{
    public MatrixModel matrixModel;
    //probability
}