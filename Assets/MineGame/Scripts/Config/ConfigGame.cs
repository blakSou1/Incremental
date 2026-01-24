using System;
using System.Linq;

[Serializable]
public class ConfigGame : EntityComponentDefinition
{
    public bool isTutorial = false;

    public CMSEntityPfb configLevelPfb;
    [NonSerialized] private ConfigLevel configLevelpr = null;

    public static string standertPiece = "PieceStandart";

    public ConfigLevel GetConfigLevel()
    {
        if (configLevelpr == null)
            configLevelpr = configLevelPfb.Components.OfType<ConfigLevel>().FirstOrDefault();
        return configLevelpr;
    }

}
