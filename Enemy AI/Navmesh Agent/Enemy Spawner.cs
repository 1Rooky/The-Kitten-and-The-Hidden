using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

// The EnemySpawner class is responsible for spawning enemies at runtime using predefined configurations and an object pooling system.
public class EnemySpawner : MonoBehaviour
{
    [SerializeField] private List<EnemySpawnerList> _enemySpawnerLists; // List of configurations for spawning different enemy types

    [SerializeField] private UnityEvent AllEnemiesDestroyed; // Event triggered when all enemies have been destroyed

    private ObjectPool _objectPool; // Reference to the ObjectPool for reusing enemy game objects
    private readonly List<GameObject> activeEnemies = new();

    // Start is called before the first frame update
    private void Start()
    {
        // Retrieve the ObjectPool instance from the ServiceLocator
        _objectPool = ServiceLocator.Instance.GetService<ObjectPool>();

        // Start the coroutine to spawn enemies for each configuration in _enemySpawnerLists
        foreach (var spawnerList in _enemySpawnerLists)
            StartCoroutine(SpawnEnemies(spawnerList));
    }

    // Coroutine to spawn enemies based on a given EnemySpawnerList configuration
    private IEnumerator SpawnEnemies(EnemySpawnerList spawnerList)
    {
        // Build the NavMesh for the area where enemies will be spawned
        spawnerList.NavMeshSurface.BuildNavMesh();

        // Delay between spawns for the current configuration
        WaitForSeconds spawnDelayWaitForSeconds = new(spawnerList.SpawnDelay);

        // Iterate over each enemy type in the configuration
        foreach (EnemyPrefabAndSize enemyPrefabAndSize in spawnerList.EnemiesPrefabs)
        {
            List<GameObject> enemiesPrefab = new();

            // Populate the list with the required number of prefabs from the object pool
            for (int i = 0; i < enemyPrefabAndSize.Size; i++)
            {
                GameObject pooledObject = _objectPool.GetPooledObject(enemyPrefabAndSize.EnemyPrefab);
                if (pooledObject != null)
                    enemiesPrefab.Add(pooledObject);
                else
                {
                    Logging.LogError("Failed to get a pooled object. Is the pool size configured correctly?");
                    yield break; // Exit the coroutine if unable to retrieve enough objects from the pool
                }
            }

            int spawnedEnemies = 0;

            // Spawn enemies until the desired number is reached
            while (spawnedEnemies < enemyPrefabAndSize.Size)
            {
                // Spawn method can be either RoundRobin or Random based on the configuration
                if (spawnerList.SpawnMethod == SpawnMethod.RoundRobin)
                    SpawnRoundRobinEnemy(spawnedEnemies, enemiesPrefab, spawnerList.NavMeshSurface);
                else if (spawnerList.SpawnMethod == SpawnMethod.Random)
                    SpawnRandomEnemy(enemiesPrefab, spawnerList.NavMeshSurface);

                spawnedEnemies++;

                // Wait for the specified delay before spawning the next enemy
                yield return spawnDelayWaitForSeconds;
            }
        }
    }

    // Spawn an enemy using a round-robin selection method
    private void SpawnRoundRobinEnemy(int spawnedEnemies, List<GameObject> enemiesPrefab, NavMeshSurface surface)
    {
        int enemyIndex = spawnedEnemies % enemiesPrefab.Count;
        DoSpawnEnemy(enemyIndex, enemiesPrefab, surface);
    }

    // Spawn an enemy at a random position
    private void SpawnRandomEnemy(List<GameObject> enemiesPrefab, NavMeshSurface surface) => DoSpawnEnemy(UnityEngine.Random.Range(0, enemiesPrefab.Count), enemiesPrefab, surface);

    // Common method for spawning an enemy
    private void DoSpawnEnemy(int enemyIndex, List<GameObject> enemiesPrefab, NavMeshSurface surface)
    {
        GameObject enemyPool = enemiesPrefab[enemyIndex];

        if (enemyPool != null && enemyPool.TryGetComponent(out Enemy enemy))
        {
            Vector3 spawnPosition = SampleNavMeshPosition(surface);

            if (spawnPosition != Vector3.zero)
            {
                enemy.NavMeshAgent.Warp(spawnPosition); // Position the enemy on the NavMesh
                enemy.NavMeshAgent.enabled = true; // Enable the NavMeshAgent
                enemy.gameObject.SetActive(true); // Activate the enemy game object
                enemy.NavmeshEnemyMovment.Spawn(); // Trigger any spawn-specific behavior

                // Add to active enemies list and subscribe to destruction event
                activeEnemies.Add(enemy.gameObject);
                enemy.OnDestroyed += () => OnEnemyDestroyed(enemy.gameObject);
            }
            else
                Logging.LogError("Failed to sample position");
        }
        else
            Logging.LogError("Enemy is null or does not have an Enemy component");
    }

    // Sample a position on the NavMesh within a specified range
    private Vector3 SampleNavMeshPosition(NavMeshSurface surface)
    {
        Vector3 center = surface.transform.position;
        float range = 10.0f; // Example range within which to sample a position

        // Attempt to find a valid position on the NavMesh within the specified range
        if (NavMesh.SamplePosition(center, out NavMeshHit hit, range, NavMesh.AllAreas))
            return hit.position;

        // Return Vector3.zero if no valid position was found
        return Vector3.zero;
    }

    // Event handler for when an enemy is destroyed
    private void OnEnemyDestroyed(GameObject enemy)
    {
        // Remove the enemy from the active enemies list
        activeEnemies.Remove(enemy);

        if (activeEnemies.Count == 0)
            AllEnemiesDestroyed?.Invoke(); // All enemies have been destroyed
    }
}