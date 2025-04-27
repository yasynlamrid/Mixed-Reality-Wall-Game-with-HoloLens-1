using UnityEngine;
using System.Collections;

public class AppearState : EnemyState
{
    private bool isAppearing = false;
    private const float EMERGE_DURATION = 2f;
    private Vector3? spawnPosition;
    private Vector3 initialScale;

    // Paramètres d'émergence
    private float blackHoleScale = 0.001f;    // Échelle initiale très petite (comme un point noir)
    private float delayBeforeEmerge = 0.2f;   // Court délai avant l'émergence
    private float distortionAmount = 0.9f;    // Effet de distorsion lors de l'émergence

    public AppearState(BaseEnemy enemy, Vector3? initialPosition = null) : base(enemy)
    {
        spawnPosition = initialPosition;
        Debug.Log($"[{enemy.gameObject.name}] AppearState initialized. Spawn position: {spawnPosition}");
    }

    public override void Enter()
    {
        Debug.Log($"[{enemy.gameObject.name}] AppearState Enter");

        if (enemy.navMeshAgent != null)
        {
            enemy.navMeshAgent.enabled = false;
        }

        initialScale = enemy.transform.localScale;

        if (spawnPosition.HasValue)
        {
            enemy.SetVisibility(false);
            ((MonoBehaviour)enemy).StartCoroutine(EmergeFromBlackHole(spawnPosition.Value));
        }
        else
        {
            Debug.LogWarning($"[{enemy.gameObject.name}] No spawn position provided!");
            enemy.ChangeState(new CommonChaseState(enemy));
        }
    }

    private IEnumerator EmergeFromBlackHole(Vector3 crackPosition)
    {
        isAppearing = true;
        Debug.Log($"[{enemy.gameObject.name}] Starting emergence at position {crackPosition}");

        // Attendre le délai initial
        yield return new WaitForSeconds(delayBeforeEmerge);

        // Position initiale exactement au point du trou noir
        enemy.transform.position = crackPosition;

        // Commencer avec une échelle minuscule (comme un point)
        enemy.transform.localScale = Vector3.one * blackHoleScale;

        // Rendre visible
        enemy.SetVisibility(true);

        if (enemy.animator != null)
        {
            enemy.animator.SetTrigger("Appear");
            Debug.Log($"[{enemy.gameObject.name}] Triggered appear animation");
        }

        float elapsed = 0f;

        while (elapsed < EMERGE_DURATION)
        {
            elapsed += Time.deltaTime;
            float progress = elapsed / EMERGE_DURATION;

            // Effet de surgissement avec Ease-Out Elastic
            float elasticProgress = EaseOutElastic(progress);

            // Échelle avec effet de distorsion
            float currentScale = Mathf.Lerp(blackHoleScale, 1f, elasticProgress);
            Vector3 scaleVector = initialScale * currentScale;

            // Ajouter un effet de distorsion pendant l'émergence
            if (progress < 0.7f)
            {
                float distortion = 1f + (Mathf.Sin(progress * Mathf.PI * 4) * (1f - progress) * distortionAmount);
                scaleVector.y *= distortion;
            }

            enemy.transform.localScale = scaleVector;

      

            yield return null;
        }

        // S'assurer que l'échelle finale est correcte
        enemy.transform.localScale = initialScale;
        Debug.Log($"[{enemy.gameObject.name}] Emergence complete");

        // Réactiver le NavMeshAgent
        if (enemy.navMeshAgent != null)
        {
            enemy.navMeshAgent.enabled = true;
        }

        isAppearing = false;
        enemy.ChangeState(new CommonChaseState(enemy));
    }

    // Fonction d'interpolation pour un effet élastique
    private float EaseOutElastic(float x)
    {
        float c4 = (2f * Mathf.PI) / 3f;

        if (x == 0f) return 0f;
        if (x == 1f) return 1f;

        return Mathf.Pow(2f, -10f * x) * Mathf.Sin((x * 10f - 0.75f) * c4) + 1f;
    }

    public override void Update()
    {
        // L'update est géré dans la coroutine
    }

    public override void Exit()
    {
        if (enemy.navMeshAgent != null && !enemy.navMeshAgent.enabled)
        {
            enemy.navMeshAgent.enabled = true;
        }
        Debug.Log($"[{enemy.gameObject.name}] AppearState Exit");
    }
}