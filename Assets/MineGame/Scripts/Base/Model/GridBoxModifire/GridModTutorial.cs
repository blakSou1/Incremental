using System.Collections;

public class GridModTutorial : GridModBase
{
    public GridModTutorial()
    {
        id = "GridModTutorial";

        Define<TagPrefabGridBoxMode>().prefab = ("prefab/GridBoxMod/" + $"{id}").Load<GridBoxMode>();
    }
    public override IEnumerator ActivationScillPlayer()
    {
        yield return null;
    }

    public override IEnumerator ActivationScillEnemy()
    {
        yield return null;
    }

}
