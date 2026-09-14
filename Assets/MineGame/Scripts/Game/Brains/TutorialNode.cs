using System;
using System.Collections.Generic;

public class TutorialNode : MatrixNode
{
    public List<ParamT> MatrixList;
    [NonSerialized] public int index = 0;

    public override Param GetMatrix()
    {
        return MatrixList[index];
    }

    public void NextIndex()
    {
        if(index < MatrixList.Count - 1)
            index++;
        else
            G.run.currentLevel++;
    }

}

[Serializable]
public class ParamT : Param
{
    public string text;
}