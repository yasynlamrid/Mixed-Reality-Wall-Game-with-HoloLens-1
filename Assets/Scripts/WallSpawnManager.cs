using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class WallSpawnManager : MonoBehaviour
{
    [Header("Spawn Settings")]
    public GameObject enemy1Prefab; // prefab pour l'ennemi1 
    public GameObject enemy2Prefab;
    public float minSpawnDelay = 1f; // la durée minimale entre les apparitions 
    public float maxSpawnDelay = 2f;
    public int enemiesPerSpawnPoint = 3; // nombre d'ennemies 
    public float spawnDistance = 0.001f; // distance de trou par rapport au mur

    [Header("Wave Settings")]
    public int maxSimultaneousEnemies = 5; // le nombre d'ennmie existant dans le jeu 
    public float timeBetweenWaves = 5f; // le temps entre deux ennemie 

    private WorldCursor worldCursor;
    private List<GameObject> activeEnemies = new List<GameObject>();
    private bool isSpawning = false;

    private class SpawnPoint
    {
        public Vector3 position;
        public Vector3 normal;
        public Vector3 spawnOffset;
        public bool isActive;
        public int enemiesSpawned;

        public SpawnPoint(Vector3 pos, Vector3 norm, Vector3 offset)
        {
            position = pos;
            normal = norm;
            spawnOffset = offset;
            isActive = true;
            enemiesSpawned = 0;
        }
    }

    private List<SpawnPoint> spawnPoints = new List<SpawnPoint>();

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
    }

    Vector3 GenerateOffset(Vector3 normal)
    {
        // Utiliser la même méthode que WorldCursor pour générer l'offset
        Vector3 tangent1 = Vector3.Cross(normal, Vector3.up).normalized;
        if (tangent1.magnitude < 0.01f)
        {
            tangent1 = Vector3.Cross(normal, Vector3.right).normalized;
        }

        Vector3 tangent2 = Vector3.Cross(normal, tangent1).normalized;
        Vector2 randomCircle = Random.insideUnitCircle * worldCursor.offsetDistance;

        return (tangent1 * randomCircle.x + tangent2 * randomCircle.y);
    }

    void OnWallClicked(Vector3 hitPoint, Vector3 normal)
    {
        // Calculer l'offset de la même manière que WorldCursor
        Vector3 offset = GenerateOffset(normal);
        Vector3 spawnPosition = hitPoint + offset + normal * spawnDistance;

        // Ajouter le nouveau point de spawn avec l'offset
        spawnPoints.Add(new SpawnPoint(spawnPosition, normal, offset));

        if (!isSpawning)
        {
            StartCoroutine(SpawnEnemiesRoutine());
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
            bool hasActivePoints = false;
            foreach (var point in spawnPoints)
            {
                if (point.isActive)
                {
                    hasActivePoints = true;
                    break;
                }
            }

            if (!hasActivePoints) break;
        }

        isSpawning = false;
    }

    void SpawnEnemy(SpawnPoint spawnPoint)
    {
        GameObject prefabToSpawn = Random.value > 0.5f ? enemy1Prefab : enemy2Prefab;

        
        Vector3 spawnPos = spawnPoint.position;

       
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
}