using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class MulticTag : EntityComponentDefinition
{
    public List<textAndImage> lines;
    [Serializable]
    public class textAndImage
    {
        public LocString text;
        public Sprite sprite;
    }
}
