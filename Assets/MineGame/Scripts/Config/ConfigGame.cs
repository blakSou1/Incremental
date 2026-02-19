using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ConfigGame : EntityComponentDefinition
{
    public bool isTutorial = false;

    [NonSerialized] private ConfigLevel configLevel = null;

    public static string standertPiece = "PieceStandart";

    public float indexLvl = 0;

    [NonSerialized] private List<(CMSEntity e, ConfigLevel tag)> list;

    public void UpdateActualLvl()
    {
        if(list == null)
            list = CMS.GetAllData<ConfigLevel>();

        foreach (var i in list)
        {
            if (i.tag.indexLvl == indexLvl)
            {
                configLevel = i.tag;

                break;
            }
        }
    }

    public ConfigLevel GetConfigLevel()
    {
        if (configLevel == null)
            UpdateActualLvl();
        return configLevel;
    }

}
