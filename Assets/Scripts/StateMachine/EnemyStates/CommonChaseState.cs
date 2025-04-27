using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class CommonChaseState : EnemyState
{
    public CommonChaseState(BaseEnemy enemy) : base(enemy) { }

    public override void Enter()
    {
        // Réinitialiser les animations
        enemy.animator.SetBool("IsRunning", true);
        enemy.animator.SetBool("IsAttacking", false);
    }

    public override void Update()
    {
        float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.position);

        if (distanceToPlayer > enemy.attackRange)
        {
            // Mode poursuite
            enemy.transform.LookAt(enemy.player);
            enemy.navMeshAgent.SetDestination(enemy.player.position);
            enemy.animator.SetBool("IsRunning", true);
        }
        else
        {
            // Arrêter de courir avant de changer d'état
            enemy.animator.SetBool("IsRunning", false);
            enemy.navMeshAgent.SetDestination(enemy.transform.position); // Arrêter le déplacement

            // Changer vers l'état d'attaque approprié
            if (enemy is Enemy1)
            {
                enemy.ChangeState(new Enemy1AttackState(enemy));
            }
            else if (enemy is Enemy2)
            {
                enemy.ChangeState(new Enemy2AttackState(enemy));
            }
            else if (enemy is Enemy3)
            {
                enemy.ChangeState(new Enemy3AttackState(enemy));
            }
        }
    }

    public override void Exit()
    {
        // S'assurer que l'animation de course est désactivée en sortant
        enemy.animator.SetBool("IsRunning", false);

        // Arrêter le NavMeshAgent
        if (enemy.navMeshAgent != null && enemy.navMeshAgent.enabled)
        {
            enemy.navMeshAgent.ResetPath();
        }
    }
}