using UnityEngine;

public class Enemy3AttackState : Enemy3State
{
    private float attackCooldownTimer;
    private bool hasShot = false;

    public Enemy3AttackState(BaseEnemy enemy) : base(enemy) { }

    public override void Enter()
    {
        if (enemy.navMeshAgent != null && enemy.navMeshAgent.enabled)
        {
            enemy.navMeshAgent.isStopped = true;
            enemy.navMeshAgent.ResetPath();
        }

        enemy.animator.SetBool("IsRunning", false);
        enemy.animator.SetBool("IsAttacking", true);

        attackCooldownTimer = enemy.attackCooldown;
        hasShot = false;
    }

    public override void Update()
    {
        attackCooldownTimer -= Time.deltaTime;
        float distanceToPlayer = Vector3.Distance(enemy.transform.position, enemy.player.position);

        if (distanceToPlayer > enemy.attackRange)
        {
            if (enemy.navMeshAgent != null)
            {
                enemy.navMeshAgent.isStopped = false;
            }
            enemy.animator.SetBool("IsAttacking", false);
            enemy.ChangeState(new CommonChaseState(enemy));
            return;
        }

        // Rotation vers le joueur
        Vector3 direction = (enemy.player.position - enemy.transform.position).normalized;
        Quaternion lookRotation = Quaternion.LookRotation(direction);
        enemy.transform.rotation = Quaternion.Slerp(
            enemy.transform.rotation,
            lookRotation,
            Time.deltaTime * 5f
        );

        // Tirer depuis tous les points si on n'a pas encore tiré
        if (!hasShot && distanceToPlayer <= enemy.attackRange)
        {
            ShootProjectilesFromAllPoints();
            hasShot = true;
        }

        if (attackCooldownTimer <= 0)
        {
            enemy.ChangeState(new Enemy3AttackState(enemy));
        }
    }

    private void ShootProjectilesFromAllPoints()
    {
        Enemy3 enemy3 = enemy as Enemy3;
        if (enemy3 != null && enemy3.projectilePrefab != null)
        {
            // Ajuster la hauteur de la cible pour viser le centre du joueur
            Vector3 targetPosition = enemy.player.position;
            targetPosition.y -= 0.5f; // Ajustement léger pour viser le centre du corps

            Vector3 direction = (targetPosition - enemy.transform.position).normalized;

            // Tirer depuis chaque point de spawn
            foreach (Transform spawnPoint in enemy3.projectileSpawnPoints)
            {
                if (spawnPoint != null)
                {
                    Vector3 spawnPosition = spawnPoint.position;
                    GameObject projectile = GameObject.Instantiate(
                        enemy3.projectilePrefab,
                        spawnPosition,
                        Quaternion.LookRotation(direction)
                    );

                    EnemyProjectile projectileScript = projectile.GetComponent<EnemyProjectile>();
                    if (projectileScript != null)
                    {
                        projectileScript.Initialize(direction);
                    }
                }
            }
        }
        else
        {
            Debug.LogWarning("Enemy3 or projectilePrefab is null");
        }
    }
    public override void Exit()
    {
        enemy.animator.SetBool("IsAttacking", false);
        if (enemy.navMeshAgent != null)
        {
            enemy.navMeshAgent.isStopped = false;
        }
    }
}