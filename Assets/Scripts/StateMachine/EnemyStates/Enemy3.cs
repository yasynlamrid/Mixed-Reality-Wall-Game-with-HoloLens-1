using UnityEngine;

public class Enemy3 : BaseEnemy
{
    public float specialAttackRange = 3f;
    public float specialAttackDamage = 20f;
    public GameObject projectilePrefab;
    public Transform[] projectileSpawnPoints; 

    protected override void Awake()
    {
        base.Awake();

        // Configurer le NavMeshAgent avec les valeurs spécifiques à Enemy2
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = true;
            navMeshAgent.speed = 0.3f;
            navMeshAgent.acceleration = 8.0f;
            navMeshAgent.stoppingDistance = 3f;
        }

        // Définir les autres paramètres spécifiques
        attackRange = 4f;
        attackCooldown = 6f;
    }

    protected override void Start()
    {
        base.Start();
    }

    // Optionnel: Méthode pour visualiser les points de spawn dans l'éditeur
#if UNITY_EDITOR
    protected override void OnDrawGizmosSelected()
    {
        base.OnDrawGizmosSelected();

        if (projectileSpawnPoints != null)
        {
            Gizmos.color = Color.cyan;
            foreach (Transform spawnPoint in projectileSpawnPoints)
            {
                if (spawnPoint != null)
                {
                    Gizmos.DrawWireSphere(spawnPoint.position, 0.2f);
                    // Dessiner la direction du tir
                    if (player != null)
                    {
                        Gizmos.DrawLine(spawnPoint.position, player.position);
                    }
                }
            }
        }
    }
#endif
}