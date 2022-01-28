using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitBox : MonoBehaviour, IDamagable
{
    protected BaseEnemy enemy;
    public bool Head;
    private Rigidbody RB;
    private void Start()
    {
        enemy = GetComponentInParent<BaseEnemy>();
        RB = GetComponent<Rigidbody>();
    }
    public void TakeDamage(int damage, Vector3 direction, Player player)
    {
        if (enemy.IsDead) return;
        if(Head)
        {
            enemy.Health -= damage * 10;
        }           
        else
        {
            enemy.Health -= damage;           
        }
        player.Points += 10;
        if (enemy.Health <= 0)
        {
            if(!Head)
            player.Points += 50;
            else
                player.Points += 100;
            RB.AddForce(direction * 1, ForceMode.Impulse);
        }
    }
}
