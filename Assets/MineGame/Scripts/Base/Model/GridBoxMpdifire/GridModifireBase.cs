using System.Collections;

public class GridModifireDamageEnemy : GridModifireBase
{
    private int damage = 1;

    public GridModifireDamageEnemy()
    {
        id = "Mod";

        Define<TagPrefabGridBoxMode>().prefab = ("prefab/GridBoxMod/" + $"{id}").Load<GridBoxMode>();
    }
    public override IEnumerator ActivationScill()
    {
        yield return G.enemyHp.StartCoroutine(G.enemyHp.Damage(damage));
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