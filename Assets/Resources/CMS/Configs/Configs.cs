using UnityEngine;
using System;

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

[Serializable]
public class ConfigGameStates : EntityComponentDefinition
{
    public bool overrideValues = false;
    public int overridePoints;
}

[Serializable]
public class ConfigLight : EntityComponentDefinition
{
    public float timeToChangeIntensity = 0.4f;
    public float timeToChangeColor = 0.4f;
    public float LightValueOne;
    public float LightValueTwo;
    public Color ColorValueOne;
    public Color ColorValueTwo;
}