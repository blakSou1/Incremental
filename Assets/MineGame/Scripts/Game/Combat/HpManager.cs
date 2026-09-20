using System.Collections;
using UnityEngine;

public class HpManager
{
	private int hp = 0;
	int damageBuf = 0;

	public void Init()
	{
        G.eventManager.SetEnemyHp.AddListener(SetHp);
        G.eventManager.Attack.AddListener(Attack);
        G.eventManager.DamagedPlayer.AddListener(i => G.eventManager.host.StartCoroutine(DamagePlayer(i)));
    }

    private void SetHp(int hp)
	{
		this.hp = hp;
		damageBuf = 0;
    }

	private void Attack(int damage)
	{
        damageBuf += damage;

        if (hp - damageBuf <= 0)
            Dead();
    }

	private IEnumerator DamagePlayer(int damage)
	{
		G.run.Damage += damage;

		G.AudioManager.PlaySound(R.Audio.damage, Random.Range(0, .5f));

		G.UIController.UpdatePlayerHp();

		if (G.run.maxHealth - G.run.Damage <= 0)
			G.eventManager.PlayerLose.Invoke();

		float target = 1 - Mathf.Min(Mathf.Max(0, (G.run.maxHealth - G.run.Damage) / G.run.maxHealth), 1);

        G.eventManager.CameraShake.Invoke(target);

        yield return G.eventManager.host.StartCoroutine(ChangeVolumeWeight(0, target));

		yield return new WaitForSeconds(.3f);

		G.eventManager.host.StartCoroutine(ChangeVolumeWeight(1, 0));

		yield return null;
	}

	private IEnumerator ChangeVolumeWeight(float startWeight = 0, float target = 1)
	{
		float elapsedTime = 0f;

		while (elapsedTime < .1f)
		{
			G.eventManager.VolumeWeugth.Invoke(Mathf.Lerp(startWeight, target, elapsedTime / .3f));
			elapsedTime += Time.deltaTime;
			yield return null;
		}

		G.eventManager.VolumeWeugth.Invoke(target);
	}

	private void Dead()
	{
		G.mainEnterPoint.StopAllCoroutines();
		G.eventManager.PlayerWin.Invoke() ;
	}
}
