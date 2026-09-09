using System.Collections;

public class GridModDamageEnemy : GridModBase
{
    private int damage = 1;

    public GridModDamageEnemy()
    {
        id = "DamageModifireBox";

        Define<TagPrefabGridBoxMode>().prefab = ("prefab/GridBoxMod/" + $"{id}").Load<GridBoxMode>();
    }
    public override IEnumerator ActivationScillPlayer()
    {
        yield return null;

        G.AudioManager.PlaySound(R.Audio.vriiis, 0);

        G.enemyHp.Damage(damage);
        G.mainEnterPoint.gameLogic.DestroyMyCoroutineSkillGridBox();
    }

    public override IEnumerator ActivationScillEnemy()
    {
        yield return null;

        yield return G.enemyHp.StartCoroutine(G.enemyHp.DamagePlayer(1));

        G.mainEnterPoint.gameLogic.DestroyMyCoroutineSkillGridBox();
    }

}