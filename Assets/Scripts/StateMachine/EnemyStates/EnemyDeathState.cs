using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeathState : EnemyState
{
    private float deathAnimationDuration = 4f;
    private float timer = 0f;

    public EnemyDeathState(BaseEnemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.animator.SetBool("IsDead", true);

        if (enemy.navMeshAgent != null)
        {
            enemy.navMeshAgent.isStopped = true;
            enemy.navMeshAgent.enabled = false;
        }
    }

    public override void Update()
    {
        timer += Time.deltaTime;

        if (timer >= deathAnimationDuration)
        {
            GameObject.Destroy(enemy.gameObject);
        }
    }

    public override void Exit()
    {
        enemy.animator.SetBool("IsDead", false);
    }
}