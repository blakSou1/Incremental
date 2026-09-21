using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConfigLevel : EntityComponentDefinition
{
    public float indexLvl = 0;

    public string preview;

    public MatrixNode matrixNode;

    public EnemyModel enemyConfig;

    public List<string> pickablePiecesId;

    public BaseBrain brain;

    public GameObject prefabRoom;

    public List<string> GetPickablePiece()
    {
        return pickablePiecesId;
    }
}
