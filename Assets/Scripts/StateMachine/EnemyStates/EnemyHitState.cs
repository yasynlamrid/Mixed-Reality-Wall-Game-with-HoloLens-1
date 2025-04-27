using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHitState : EnemyState
{
    private float hitAnimationDuration = 1f; // Ajustez selon votre animation
    private float timer = 0f;

    public EnemyHitState(BaseEnemy enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.animator.SetBool("IsHit", true);

        // Arrêter le déplacement pendant l'animation de hit
        if (enemy.navMeshAgent != null)
        {
            enemy.navMeshAgent.isStopped = true;
        }
    }

    public override void Update()
    {
        timer += Time.deltaTime;

        if (timer >= hitAnimationDuration)
        {
            // Retourner à l'état de poursuite après l'animation
            enemy.ChangeState(new CommonChaseState(enemy));
        }
    }

    public override void Exit()
    {
        enemy.animator.SetBool("IsHit", false);
        if (enemy.navMeshAgent != null)
        {
            enemy.navMeshAgent.isStopped = false;
        }
    }
}