using System.Collections;
using UnityEngine;

public class DamageEnemyScenario : MonoBehaviour
{
    public ParticleSystem vfx;

    public void Start()
    {
        G.DamageEnemyScenario = this;
    }
    
    public IEnumerator StartScenario()
    {
        yield return StartCoroutine(G.PlayerController.MoveAndRotate(G.PlayerController.cameraPositionordinary, G.PlayerController.cameraPositionVisibalEnemy));

        while(G.enemyHp.GetHp() > 0 && G.enemyHp.GetBuffer() > 0)
        {
            G.AudioManager.PlaySound(R.Audio.damage, Random.Range(.2f, .6f));

            G.enemyHp.ActivDamage(G.configGame.damagePlayer);
            var vfxL = Instantiate(vfx.gameObject, G.enemySprite.transform).GetComponent<ParticleSystem>();

            StartCoroutine(StopVFX(vfxL));

            yield return new WaitForSeconds(.2f);
        }

        yield return new WaitForSeconds(.2f);

        G.enemyHp.WhatDead();
    }

    private IEnumerator StopVFX(ParticleSystem particleSystem)
    {
        yield return new WaitForSeconds(.5f);
        particleSystem.Stop();
    }
}
