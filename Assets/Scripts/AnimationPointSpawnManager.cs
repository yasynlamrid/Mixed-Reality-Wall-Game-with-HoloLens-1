using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AnimationPointSpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemy1Prefab;
    public GameObject enemy2Prefab;
    public float minSpawnDelay = 1f;
    public float maxSpawnDelay = 2f;
    public int enemiesPerSpawnPoint = 3;

    [Header("Wave Settings")]
    public int maxSimultaneousEnemies = 5;
    public float timeBetweenWaves = 5f;

    private WorldCursor worldCursor;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool isSpawning = false;

    private class SpawnPointData
    {
        public Vector3 spawnPosition;  // Position exacte de l'animation
        public Vector3 normal;         // Normale du mur
        public bool isActive;
        public int enemiesSpawned;

        public SpawnPointData(Vector3 pos, Vector3 norm)
        {
            spawnPosition = pos;
            normal = norm;
            isActive = true;
            enemiesSpawned = 0;
        }
    }

    private List<SpawnPointData> spawnPoints = new List<SpawnPointData>();

    void Start()
    {
        worldCursor = FindObjectOfType<WorldCursor>();
        if (worldCursor != null)
        {
            worldCursor.OnWallClicked += OnWallClicked;
        }
        else
        {
            Debug.LogError("WorldCursor non trouvé dans la scène!");
        }

        if (AnimationPointsManager.Instance == null)
        {
            Debug.LogError("AnimationPointsManager non trouvé!");
        }
    }

    void OnWallClicked(Vector3 hitPoint, Vector3 normal)
    {
        // Attendre que l'AnimationPointsManager enregistre les points
        StartCoroutine(WaitForAnimationPoints(hitPoint, normal));
    }

    IEnumerator WaitForAnimationPoints(Vector3 hitPoint, Vector3 normal)
    {
        // Attendre un frame pour que l'AnimationPointsManager enregistre les points
        yield return new WaitForEndOfFrame();

        if (AnimationPointsManager.Instance != null)
        {
            // Obtenir le point de spawn exact à partir de l'AnimationPointsManager
            Vector3 spawnPosition = AnimationPointsManager.Instance.GetSpawnPosition();

            // Ajouter le nouveau point de spawn
            spawnPoints.Add(new SpawnPointData(spawnPosition, normal));

            if (!isSpawning)
            {
                StartCoroutine(SpawnEnemiesRoutine());
            }
        }
    }

    IEnumerator SpawnEnemiesRoutine()
    {
        isSpawning = true;

        while (spawnPoints.Count > 0)
        {
            CleanupDeadEnemies();

            if (activeEnemies.Count < maxSimultaneousEnemies)
            {
                foreach (var spawnPoint in spawnPoints.ToArray())
                {
                    if (!spawnPoint.isActive || spawnPoint.enemiesSpawned >= enemiesPerSpawnPoint)
                        continue;

                    // Attendre que l'animation de la fissure soit terminée
                    yield return new WaitForSeconds(worldCursor.crackDuration);

                    SpawnEnemy(spawnPoint);
                    spawnPoint.enemiesSpawned++;

                    if (spawnPoint.enemiesSpawned >= enemiesPerSpawnPoint)
                    {
                        spawnPoint.isActive = false;
                    }

                    yield return new WaitForSeconds(Random.Range(minSpawnDelay, maxSpawnDelay));
                }
            }

            yield return new WaitForSeconds(0.5f);

            // Vérifier s'il reste des points actifs
            if (!spawnPoints.Exists(p => p.isActive)) break;
        }

        isSpawning = false;
    }

    void SpawnEnemy(SpawnPointData spawnPoint)
    {
        GameObject prefabToSpawn = Random.value > 0.5f ? enemy1Prefab : enemy2Prefab;

        // Utiliser la position exacte de l'animation
        Vector3 spawnPos = spawnPoint.spawnPosition;

        // Rotation pour faire face à l'opposé du mur
        Quaternion rotation = Quaternion.LookRotation(-spawnPoint.normal);

        GameObject enemy = Instantiate(prefabToSpawn, spawnPos, rotation);
        activeEnemies.Add(enemy);

        BaseEnemy baseEnemy = enemy.GetComponent<BaseEnemy>();
        if (baseEnemy != null)
        {
            baseEnemy.ChangeState(new AppearState(baseEnemy, spawnPos));
        }

        EnemyDeathTracker deathTracker = enemy.AddComponent<EnemyDeathTracker>();
        deathTracker.OnEnemyDestroyed += () => OnEnemyDestroyed(enemy);
    }

    void OnEnemyDestroyed(GameObject enemy)
    {
        if (activeEnemies.Contains(enemy))
        {
            activeEnemies.Remove(enemy);
        }
    }

    void CleanupDeadEnemies()
    {
        activeEnemies.RemoveAll(enemy => enemy == null);
    }

    void OnDestroy()
    {
        if (worldCursor != null)
        {
            worldCursor.OnWallClicked -= OnWallClicked;
        }
    }












    // Dans AnimationPointSpawnManager.cs, gardez seulement :
    public void ResetSpawnManager()
    {
        StopAllCoroutines();
        isSpawning = false;

        // Détruire tous les ennemis actifs
        foreach (var enemy in activeEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        activeEnemies.Clear();
        spawnPoints.Clear();
        Debug.Log("SpawnManager réinitialisé");
    }







    // Pour le debug visuel
    void OnDrawGizmos()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            if (spawnPoint.isActive)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(spawnPoint.spawnPosition, 0.1f);
                Gizmos.DrawRay(spawnPoint.spawnPosition, spawnPoint.normal);
            }
        }
    }
}