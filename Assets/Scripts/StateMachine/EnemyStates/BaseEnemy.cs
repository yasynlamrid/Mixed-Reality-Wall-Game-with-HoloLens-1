using UnityEngine;
using UnityEngine.AI;

public abstract class BaseEnemy : MonoBehaviour
{
    public NavMeshAgent navMeshAgent;
    public Animator animator;
    [SerializeField]
    private Transform _player;
    public bool useMainCameraAsPlayer = true;
    public float attackRange = 4f;
    public float attackCooldown = 1.5f;
    protected EnemyState currentState;
    private MeshRenderer meshRenderer;
    private SkinnedMeshRenderer skinnedMeshRenderer;
    public int maxHealth = 2;
    protected int currentHealth;

    public Transform player
    {
        get
        {
            if (_player == null && useMainCameraAsPlayer)
            {
                _player = Camera.main.transform;
            }
            return _player;
        }
        set { _player = value; }
    }

    public virtual void ChangeState(EnemyState newState)
    {
        if (currentState != null)
            currentState.Exit();
        currentState = newState;
        currentState.Enter();
    }

    protected virtual void Awake()
    {
        Debug.Log("Enemy Awake called");
        // Initialiser les composants
        navMeshAgent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        meshRenderer = GetComponent<MeshRenderer>();
        skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();

        // Configurer le NavMeshAgent avec des valeurs par défaut
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = true;

            // Ces valeurs seront utilisées uniquement si elles ne sont pas surchargées
            // dans les classes dérivées
            if (navMeshAgent.speed == 0)
                navMeshAgent.speed = 3.5f;
            if (navMeshAgent.acceleration == 0)
                navMeshAgent.acceleration = 8.0f;
            if (navMeshAgent.stoppingDistance == 0)
                navMeshAgent.stoppingDistance = 1.0f;
        }
    }
    protected virtual void Start()
    {
        currentHealth = maxHealth;
        Debug.Log("Enemy Start called");
        if (useMainCameraAsPlayer && _player == null)
        {
            _player = Camera.main.transform;
            if (_player == null)
            {
                Debug.LogError("No MainCamera found in the scene!");
            }
        }

        // S'assurer que l'ennemi est visible
        SetVisibility(true);

        // Démarrer avec l'état d'apparition
        Debug.Log("Starting with AppearState");
        ChangeState(new AppearState(this));
    }

    protected virtual void Update()
    {
        if (currentState != null)
        {
            currentState.Update();
        }
        else
        {
            Debug.LogWarning("No current state in enemy update!");
        }

        // Vérifier si le player est toujours assigné
        if (player == null && useMainCameraAsPlayer)
        {
            _player = Camera.main.transform;
        }
    }

    public virtual void SetVisibility(bool isVisible)
    {
        Debug.Log($"Setting visibility to {isVisible} for enemy {gameObject.name}");

        // Gérer le MeshRenderer principal
        if (meshRenderer != null)
        {
            meshRenderer.enabled = isVisible;
            Debug.Log($"Main MeshRenderer enabled: {meshRenderer.enabled}");
        }

        // Gérer le SkinnedMeshRenderer principal
        if (skinnedMeshRenderer != null)
        {
            skinnedMeshRenderer.enabled = isVisible;
            Debug.Log($"Main SkinnedMeshRenderer enabled: {skinnedMeshRenderer.enabled}");
        }

        // Gérer tous les MeshRenderers enfants
        MeshRenderer[] childRenderers = GetComponentsInChildren<MeshRenderer>(true);
        foreach (MeshRenderer renderer in childRenderers)
        {
            if (renderer != meshRenderer) // Éviter de configurer deux fois le renderer principal
            {
                renderer.enabled = isVisible;
                Debug.Log($"Child MeshRenderer {renderer.gameObject.name} enabled: {renderer.enabled}");
            }
        }

        // Gérer tous les SkinnedMeshRenderers enfants
        SkinnedMeshRenderer[] childSkinnedRenderers = GetComponentsInChildren<SkinnedMeshRenderer>(true);
        foreach (SkinnedMeshRenderer renderer in childSkinnedRenderers)
        {
            if (renderer != skinnedMeshRenderer) // Éviter de configurer deux fois le renderer principal
            {
                renderer.enabled = isVisible;
                Debug.Log($"Child SkinnedMeshRenderer {renderer.gameObject.name} enabled: {renderer.enabled}");
            }
        }
    }

    // Méthode utilitaire pour vérifier si l'agent est sur le NavMesh
    protected bool IsAgentOnNavMesh()
    {
        if (navMeshAgent == null) return false;

        NavMeshHit hit;
        return NavMesh.SamplePosition(transform.position, out hit, 1.0f, NavMesh.AllAreas);
    }

    protected virtual void OnEnable()
    {
        SetVisibility(true);
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = true;
        }
    }

    protected virtual void OnDisable()
    {
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = false;
        }
    }

    // Méthode pour réinitialiser l'ennemi si nécessaire
    public virtual void ResetEnemy()
    {
        if (currentState != null)
        {
            currentState.Exit();
        }
        SetVisibility(true);
        if (navMeshAgent != null)
        {
            navMeshAgent.enabled = true;
        }
        ChangeState(new AppearState(this));
    }


    public virtual void TakeDamage(int damage)
    {
        currentHealth -= damage;

        if (currentHealth == 1) // Premier coup
        {
            // Changer vers l'état "touché"
            ChangeState(new EnemyHitState(this));
        }
        else if (currentHealth <= 0) // Deuxième coup
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        ChangeState(new EnemyDeathState(this));
    }



#if UNITY_EDITOR
    // Méthodes de debug visuel dans l'éditeur
    protected virtual void OnDrawGizmosSelected()
    {
        // Dessiner la range d'attaque
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);

        
        if (player != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, player.position);
        }
    }
#endif
}