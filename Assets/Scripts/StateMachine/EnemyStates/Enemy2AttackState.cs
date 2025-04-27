using UnityEngine;

public class Enemy2AttackState : Enemy2State
{
    private float attackCooldownTimer;
    private bool hasShot = false;

    public Enemy2AttackState(BaseEnemy enemy) : base(enemy) { }

    public override void Enter()
    {
        // Arrêter le déplacement
        if (enemy.navMeshAgent != null && enemy.navMeshAgent.enabled)
        {
            enemy.navMeshAgent.isStopped = true;
            enemy.navMeshAgent.ResetPath();
        }

        // Configurer les animations
        enemy.animator.SetBool("IsRunning", false);
        enemy.animator.SetBool("IsAttacking", true);

        // Initialiser les variables d'attaque
        attackCooldownTimer = enemy.attackCooldown;
        hasShot = false;
    }

    public override void Update()
    {
        attackCooldownTimer -= Time.deltaTime;
        float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.position);

        // Si trop loin, retourner à la poursuite
        if (distanceToPlayer > enemy.attackRange)
        {
            // Réactiver le déplacement
            if (enemy.navMeshAgent != null)
            {
                enemy.navMeshAgent.isStopped = false;
            }

            enemy.animator.SetBool("IsAttacking", false);
            enemy.ChangeState(new CommonChaseState(enemy));
            return;
        }

        // Rotation vers le joueur sur place
        Vector3 direction = (enemy.player.position - enemy.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        enemy.transform.rotation = Quaternion.Slerp(
            enemy.transform.rotation,
            lookRotation,
            Time.deltaTime * 5f
        );

        // Tirer si on n'a pas encore tiré
        if (!hasShot && distanceToPlayer <= enemy.attackRange)
        {
            ShootProjectile();
            hasShot = true;
        }

        // Réinitialiser l'attaque après le cooldown
        if (attackCooldownTimer <= 0)
        {
            enemy.ChangeState(new Enemy2AttackState(enemy));
        }
    }

    private void ShootProjectile()
    {
        Enemy2 enemy2 = enemy as Enemy2;
        if (enemy2 != null && enemy2.projectilePrefab != null)
        {
            // Créer un point de ciblage plus bas
            Vector3 targetPosition = enemy.player.position;
            targetPosition.y -= 1f; // Réduire la hauteur de visée de 1 unité (ajustez selon vos besoins)

            Vector3 direction = (targetPosition - enemy.transform.position).normalized;
            Vector3 spawnPosition = enemy2.projectileSpawnPoint != null
                ? enemy2.projectileSpawnPoint.position
                : enemy.transform.position + direction + Vector3.up * 0.5f;

            GameObject projectile = GameObject.Instantiate(
                enemy2.projectilePrefab,
                spawnPosition,
                Quaternion.LookRotation(direction)
            );

            projectile.GetComponent<EnemyProjectile>()?.Initialize(direction);
        }
    }
    public override void Exit()
    {
        // Réinitialiser les animations
        enemy.animator.SetBool("IsAttacking", false);

        // Réactiver le déplacement
        if (enemy.navMeshAgent != null)
        {
            enemy.navMeshAgent.isStopped = false;
        }
    }
}