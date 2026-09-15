using System;
using System.Collections.Generic;

[Serializable]
public class ConfigGame : EntityComponentDefinition
{
    [NonSerialized] private ConfigLevel configLevel = null;
    [NonSerialized] public MatrixModel MatrixModel = null;

    public static string standardPiece = "PieceStandart";

    [NonSerialized] private List<(CMSEntity e, ConfigLevel tag)> list;

    public void UpdateActualLvl()
    {
        if(list == null)
            list = CMS.GetAllData<ConfigLevel>();

        foreach (var i in list)
        {
            if (i.tag.indexLvl == G.run.currentLevel)
            {
                configLevel = i.tag;

                break;
            }
        }
    }

    public ConfigLevel GetConfigLevel()
    {
        if (configLevel == null || configLevel.indexLvl != G.run.currentLevel)
            UpdateActualLvl();
        return configLevel;
    }

}
