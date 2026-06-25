using System;
using System.Collections.Generic;

[Serializable]
public class ConfigGame : EntityComponentDefinition
{
    public bool isTutorial = false;

    [NonSerialized] private ConfigLevel configLevel = null;
    [NonSerialized] public MatrixModel MatrixModel = null;

    public static string standertPiece = "PieceStandart";
    public static string damageModBox = "DamageModifireBox";

    public int indexWinLvl = 2;

    public int damagePlayer = 1;

    [NonSerialized] private List<(CMSEntity e, ConfigLevel tag)> list;

    public void UpdateActualLvl()
    {
        if(list == null)
            list = CMS.GetAllData<ConfigLevel>();

        foreach (var i in list)
        {
            if (i.tag.indexLvl == G.run.indexLvl)
            {
                configLevel = i.tag;

                break;
            }
        }
    }

    public ConfigLevel GetConfigLevel()
    {
        if (configLevel == null || configLevel.indexLvl != G.run.indexLvl)
            UpdateActualLvl();
        return configLevel;
    }

}
