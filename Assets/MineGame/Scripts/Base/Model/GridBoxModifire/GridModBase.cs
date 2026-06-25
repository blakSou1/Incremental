using System.Collections;

public abstract class GridModBase : CMSEntity
{
    public GridModBase()
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