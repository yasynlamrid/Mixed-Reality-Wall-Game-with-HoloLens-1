using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Enemy1AttackState.cs
public class Enemy1AttackState : Enemy1State
{
    private float attackCooldownTimer;

    public Enemy1AttackState(BaseEnemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.animator.SetTrigger("Attack1");
        attackCooldownTimer = enemy.attackCooldown;
    }

    public override void Update()
    {
        attackCooldownTimer -= Time.deltaTime;

        Vector3 direction = (enemy.player.position - enemy.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        enemy.transform.rotation = Quaternion.Slerp(
            enemy.transform.rotation,
            lookRotation,
            Time.deltaTime * 5f
        );

        if (Vector3.Distance(enemy.transform.position, enemy.player.position) > enemy.attackRange)
        {
            enemy.ChangeState(new CommonChaseState(enemy));
        }
        else if (attackCooldownTimer <= 0)
        {
            enemy.ChangeState(new Enemy1AttackState(enemy));
        }
    }

    public override void Exit() { }
}