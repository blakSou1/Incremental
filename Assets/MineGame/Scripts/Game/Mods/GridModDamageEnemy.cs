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

        G.eventManager.DamagedPlayer.Invoke(damage);
    }

    public override IEnumerator ActivationScillEnemy()
    {
        yield return null;

        G.eventManager.Attack.Invoke(1);
    }

}