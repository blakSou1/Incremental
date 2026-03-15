using System;
using UnityEngine;

[Serializable]
public class ConfigEnemy : EntityComponentDefinition
{
    public AnimationDataSO enemySpawnAnim;
    public AnimationDataSO enemyWinAnim;
    public AnimationDataSO enemyLouseAnim;

    public Vector2 Scale = new(1, 1);

    public int hp = 0;
}
