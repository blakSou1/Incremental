using System;
using UnityEngine;

[Serializable]
public class ConfigMain : EntityComponentDefinition
{
#if UNITY_EDITOR
    [Header("Fade settings")]
    public bool showFading = true;
    [Header("Localization settings")]
    public bool showLocOnStart = true;
    public bool RewriteLocWithRus = false;
#else
        [HideInInspector] public bool showFading = true;
        [HideInInspector] public bool showLocOnStart = true;
        [HideInInspector] public bool RewriteLocWithRus = false;
#endif
}