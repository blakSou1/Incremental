using System;
using System.Linq;

[Serializable]
public class ConfigLevel : EntityComponentDefinition
{
    public CMSEntityPfb enemyConfigPfb;
    [NonSerialized] private ConfigEnemy enemyConfig;

    public ConfigEnemy GetConfigEnemy()
    {
        if (enemyConfig == null)
            enemyConfig = enemyConfigPfb.Components.OfType<ConfigEnemy>().FirstOrDefault();
        return enemyConfig;
    }

}
