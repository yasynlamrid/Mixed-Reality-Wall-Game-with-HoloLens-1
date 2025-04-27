
using UnityEngine;
public class Enemy2 : BaseEnemy
{
    public float specialAttackRange = 3f;
    public float specialAttackDamage = 20f;
    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;

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
}