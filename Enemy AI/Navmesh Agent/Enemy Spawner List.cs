using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

// Class to store a list of enemy spawners
[System.Serializable]
public class EnemySpawnerList
{
    [SerializeField] private List<EnemyPrefabAndSize> _enemiesPrefabs; // List of enemy prefabs
    [SerializeField] private NavMeshSurface _navMeshSurface; // NavMesh surface
    [SerializeField] private float _spawnDelay = 1f; // Delay between spawns
    [SerializeField] private SpawnMethod _spawnMethod = SpawnMethod.RoundRobin; // Method to spawn enemies

    // Getters
    internal List<EnemyPrefabAndSize> EnemiesPrefabs => _enemiesPrefabs;

    internal NavMeshSurface NavMeshSurface => _navMeshSurface;

    internal float SpawnDelay => _spawnDelay;

    internal SpawnMethod SpawnMethod => _spawnMethod;
}

[System.Serializable]
public class EnemyPrefabAndSize
{
    [SerializeField] private GameObject _enemyPrefab; // Enemy prefab

    [SerializeField] private int _size; // Size of the pool

    internal GameObject EnemyPrefab => _enemyPrefab;

    internal int Size => _size;
}