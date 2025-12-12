using System;
using System.Linq;

[Serializable]
public class ConfigGame : EntityComponentDefinition
{
    public bool isTutorial = false;

    public CMSEntityPfb configLevel;
    [NonSerialized] private ConfigLevel configLevelpr = null;

    public ConfigLevel GetConfigLevel()
    {
        if(configLevelpr == null)
            configLevelpr = configLevel.Components.OfType<ConfigLevel>().FirstOrDefault();
        return configLevelpr;
    }

}
