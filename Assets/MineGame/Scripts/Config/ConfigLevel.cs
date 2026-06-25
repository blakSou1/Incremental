using System;
using System.Collections.Generic;

[Serializable]
public class ConfigLevel : EntityComponentDefinition
{
    public float indexLvl = 0;

    public LocString preview;

    public MatrixNode matrixNode;

    public EnemyModel enemyConfig;

    public List<string> pickablePiecesId;

    public BaseBrain brain;

    public List<string> GetPickablePiece()
    {
        return pickablePiecesId;
    }
}
