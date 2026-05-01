using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class ConfigLevel : EntityComponentDefinition
{
    public float indexLvl = 0;

    public string preview;

    public CMSEntityPfb enemyConfigPfb;
    [NonSerialized] private ConfigEnemy enemyConfig;

    public List<string> pickablePiecesId;

    public ConfigEnemy GetConfigEnemy()
    {
        if (enemyConfig == null)
            enemyConfig = enemyConfigPfb.Components.OfType<ConfigEnemy>().FirstOrDefault();
        return enemyConfig;
    }

    public List<string> GetPickablePiece()
    {
        return pickablePiecesId;
    }
}
