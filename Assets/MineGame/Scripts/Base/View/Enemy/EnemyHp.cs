using System.Collections;
using TMPro;
using UnityEngine;

public class EnemyHp : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI damageIndicator;

    [HideInInspector] public int damageBuffer;

    private int hp = 0;

    public void Start()
    {
        G.enemyHp = this;
        damageIndicator.text = "";

        SetHp(G.configGame.GetConfigLevel().GetConfigEnemy().hp);
    }

    public void SetHp(int hp)
    {
        this.hp = hp;
    }
    public int GetHp()
    {
        return hp;
    }
    public int GetBuffer()
    {
        return damageBuffer;
    }

    public void Damage(int damage)
    {
        damageBuffer += damage;

        damageIndicator.text = $"{damageBuffer}";
    }
    public void ActivDamage(int damage)
    {
        hp -= damage;//TODO Remove 1 piece

        damageBuffer -= damage;

        damageIndicator.text = $"{damageBuffer}";

        if (hp <= 0)
            ;//TODO Add Money
    }

    public bool WhatDead()
    {
        if (hp <= 0)
        {
            Dead();
            return true;
        }

        return false;
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
        G.mainEnterPoint.StopAllCoroutines();
        G.winAndLouse.WinPlayer();
    }
}
