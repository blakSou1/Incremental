using UnityEngine;

public class EnemyModel : MonoBehaviour
{
    public AnimationClip enemySpawnAnim;
    public AnimationClip enemyWinAnim;
    public AnimationClip enemyLouseAnim;

    public Vector2 Scale = new(1, 1);

    public int hp = 0;
}
