using System.Collections;

public class GridModifireDamageEnemy : GridModifireBase
{
    private int damage = 1;

    public GridModifireDamageEnemy()
    {
        id = "DamageModifireBox";

        Define<TagPrefabGridBoxMode>().prefab = ("prefab/GridBoxMod/" + $"{id}").Load<GridBoxMode>();
    }
    public override IEnumerator ActivationScillPlayer()
    {
        yield return null;

        G.AudioManager.PlaySound(R.Audio.vriiis, 0);

        G.enemyHp.Damage(damage);
        G.gameLogic.DestroyMyCoroutineSkillGridBox();
    }

    public override IEnumerator ActivationScillEnemy()
    {
        yield return null;

        yield return G.enemyHp.StartCoroutine(G.enemyHp.DamagePlayer(1));

        G.gameLogic.DestroyMyCoroutineSkillGridBox();
    }

}

public abstract class GridModifireBase : CMSEntity
{
    public GridModifireBase()
    {
        Define<TagPrefabGridBoxMode>().prefab = "prefab/GridBoxMod/name".Load<GridBoxMode>();
        id = "name";
    }

    public virtual IEnumerator ActivationScillPlayer()
    {
        yield return null;
    }
    public virtual IEnumerator ActivationScillEnemy()
    {
        yield return null;
    }
}

public class TagPrefabGridBoxMode : EntityComponentDefinition
{
    public GridBoxMode prefab;
}