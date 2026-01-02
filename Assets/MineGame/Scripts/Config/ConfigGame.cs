using System;
using System.Collections.Generic;
using System.Linq;

[Serializable]
public class ConfigGame : EntityComponentDefinition
{
    public bool isTutorial = false;

    public CMSEntityPfb configLevelPfb;
    [NonSerialized] private ConfigLevel configLevelpr = null;

    public Piece standertPiece;

    [NonSerialized] public List<Piece> piece;
    public List<Piece> pieces;

    public ConfigLevel GetConfigLevel()
    {
        if (configLevelpr == null)
            configLevelpr = configLevelPfb.Components.OfType<ConfigLevel>().FirstOrDefault();
        return configLevelpr;
    }

}
