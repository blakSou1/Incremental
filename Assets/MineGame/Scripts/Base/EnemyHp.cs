using UnityEngine;

public class EnemyHp : MonoBehaviour
{
    private int hp = 0;

    public void Start()
    {
        G.enemyHp = this;

        G.UIController.UpdateHpEnemy(1);
    }

    public void SetHp(int hp)
    {
        this.hp = hp;
    }

    public void Damage(int damage)
    {
        hp -= damage;

        G.UIController.UpdateHpEnemy(hp);

        if (hp <= 0)
            Dead();
    }

    private void Dead()
    {
        G.gameMode.StopAllCoroutines();
        G.winAndLouse.WinPlayer();
    }
}
