using System.Collections.Generic;
using UnityEngine;

// The ObjectPool class manages a pool of objects for reuse, reducing the overhead associated with instantiating and destroying objects frequently.
public class ObjectPool : MonoBehaviour
{
    [SerializeField] internal List<Pool> pools; // List of different object pools
    [SerializeField] internal Dictionary<GameObject, List<GameObject>> poolDictionary; // Dictionary mapping prefabs to their respective pools

    // Awake is called when the script instance is being loaded
    private void Awake()
    {
        // Register this ObjectPool instance with the ServiceLocator for easy access from other scripts
        ServiceLocator.Instance.RegisterService(this, false);

        // Initialize the dictionary that will hold the pools
        poolDictionary = new();
    }

    // Start is called before the first frame update
    private void Start() => InitializeList();

    // Initializes the object pools based on the 'pools' list
    private void InitializeList()
    {
        foreach (Pool pool in pools)
        {
            List<GameObject> objectPool = new();

            // Instantiate and deactivate the initial objects for the pool
            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Add(obj);
            }

            // Add the newly created pool to the dictionary
            poolDictionary.Add(pool.prefab, objectPool);
        }
    }

    // Retrieves an object from the specified pool, or extends the pool if necessary
    internal GameObject GetPooledObject(GameObject prefab)
    {
        // Clean up the prefab name to match keys in the dictionary
        string prefabName = prefab.name.Replace("(Clone)", "").Trim();

        // Find the original prefab in the dictionary
        GameObject originalPrefab = null;
        foreach (var key in poolDictionary.Keys)
        {
            if (key.name == prefabName)
            {
                originalPrefab = key;
                break;
            }
        }

        if (originalPrefab == null)
        {
            Logging.LogError($"ObjectPool does not contain a pool for prefab: {prefab.name}");
            return null;
        }

        // Retrieve an inactive object from the pool
        foreach (GameObject obj in poolDictionary[originalPrefab])
        {
            if (!obj.activeInHierarchy)
            {
                obj.SetActive(true);
                return obj;
            }
        }

        // If no inactive object is available, extend the pool
        GameObject newObj = ExtendObjects(originalPrefab);
        if (newObj != null)
        {
            Logging.Log($"Extended pool for prefab: {originalPrefab.name}");
            newObj.SetActive(true);
            return newObj;
        }

        Logging.LogError($"Failed to extend pool for prefab: {originalPrefab.name}. Check if the prefab is correctly assigned in the pool.");
        return null;
    }

    // Extends the pool by instantiating a new object
    private GameObject ExtendObjects(GameObject prefab)
    {
        // Find the corresponding pool for the prefab
        Pool pool = null;
        for (int i = 0; i < pools.Count; i++)
        {
            if (pools[i].prefab == prefab)
            {
                pool = pools[i];
                break;
            }
        }

        // Instantiate a new object and add it to the pool
        if (pool != null)
        {
            GameObject obj = Instantiate(pool.prefab);
            obj.SetActive(false);
            pool.pooledObjects.Add(obj);
            poolDictionary[prefab].Add(obj);
            return obj;
        }

        return null;
    }

    // Returns an object to the pool by deactivating it
    internal void ReturnToPool(int _, GameObject objectToReturn) => objectToReturn.SetActive(false);

    // Retrieves the size of a specific pool
    internal int GetPoolSize(GameObject prefab)
    {
        if (!poolDictionary.ContainsKey(prefab))
            return -1;

        return poolDictionary[prefab].Count;
    }

    // Retrieves the list of pooled objects for a specific prefab
    internal List<GameObject> GetPooledObjects(GameObject prefab)
    {
        if (!poolDictionary.ContainsKey(prefab))
            return null;

        return poolDictionary[prefab];
    }
}