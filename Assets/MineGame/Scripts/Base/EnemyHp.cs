using System.Collections;
using UnityEngine;
using UnityEngine.Rendering;
using static UnityEngine.GraphicsBuffer;

public class EnemyHp : MonoBehaviour
{
    private int hp = 0;

    public void Start()
    {
        G.enemyHp = this;

        SetHp(G.configGame.GetConfigLevel().GetConfigEnemy().hp);
    }

    public void SetHp(int hp)
    {
        this.hp = hp;
    }

    public IEnumerator Damage(int damage)
    {
        hp -= damage;

        if (hp <= 0)
            Dead();

        yield return null;
    }
    public IEnumerator DamagePlayer(int damage)
    {
        G.run.health -= damage;

        G.AudioManager.PlaySound(R.Audio.damage, Random.Range(0, .5f));

        G.UIController.UpdatePlayerHp();

        if (G.run.health <= 0)
            G.winAndLouse.WinEnemy();

        yield return StartCoroutine(ChangeVolumeWeight(1 - Mathf.Min(Mathf.Max(0, G.run.health / G.run.maxHealth), 1)));

        yield return new WaitForSeconds(.3f);

        StartCoroutine(ChangeVolumeWeight(0));

        yield return null;
    }
    private IEnumerator ChangeVolumeWeight(float target = 1)
    {
        float startWeight = G.louse.volume.weight;
        float elapsedTime = 0f;

        while (elapsedTime < .1f)
        {
            G.louse.volume.weight = Mathf.Lerp(startWeight, target, elapsedTime / .3f);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        G.louse.volume.weight = target;
    }


    private void Dead()
    {
        G.gameMode.StopAllCoroutines();
        G.winAndLouse.WinPlayer();
    }
}
