using System.Collections;

public class GridModifireDamageEnemy : GridModifireBase
{
    private int damage = 1;

    public GridModifireDamageEnemy()
    {
        id = "DamageModifireBox";

        Define<TagPrefabGridBoxMode>().prefab = ("prefab/GridBoxMod/" + $"{id}").Load<GridBoxMode>();
    }
    public override IEnumerator ActivationScill()
    {
        yield return null;

        G.enemyHp.Damage(damage);
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

    public virtual IEnumerator ActivationScill()
    {
        yield return null;
    }
}

public class TagPrefabGridBoxMode : EntityComponentDefinition
{
    public GridBoxMode prefab;
}