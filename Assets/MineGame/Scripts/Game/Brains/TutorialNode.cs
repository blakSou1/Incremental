using System;
using System.Collections.Generic;

public class TutorialNode : MatrixNode
{
    public Param MatrixList;

    public override Param GetMatrix()
    {
        return MatrixList;
    }

}
