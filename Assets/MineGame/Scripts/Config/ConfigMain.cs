using System;
using UnityEngine;

[Serializable]
public class ConfigMain : EntityComponentDefinition
{
    [HideInInspector] public bool showFading = true;
}