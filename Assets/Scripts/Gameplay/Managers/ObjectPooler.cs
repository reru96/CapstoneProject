using System.Collections.Generic;
using UnityEngine;
using System;
using Core;
using Gameplay;
[Serializable]
public class PoolEntry
{
    public string name;          
    public GameObject prefab;     
    public int size = 10;        
}
public class ObjectPooler : Injectable<ObjectPooler>
{
    [SerializeField] private List<PoolEntry> poolEntries = new List<PoolEntry>();
    private Dictionary<GameObject, Queue<GameObject>> poolDictionary = new Dictionary<GameObject, Queue<GameObject>>();

    protected override void Awake()
    {
        base.Awake();
        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var entry in poolEntries)
        {
            if (entry.prefab == null) continue;
            AddToPool(entry.prefab, entry.size);
        }
    }

    public void AddToPool(GameObject prefab, int size)
    {
        if (prefab == null) return;

        if (!poolDictionary.ContainsKey(prefab))
            poolDictionary[prefab] = new Queue<GameObject>();

        var queue = poolDictionary[prefab];

        for (int i = 0; i < size; i++)
        {
            GameObject obj = Instantiate(prefab);
            obj.SetActive(false);

            if (obj.GetComponent<Poolable>() == null)
                obj.AddComponent<Poolable>();

            queue.Enqueue(obj);
        }
    }

    public bool HasPrefab(GameObject prefab)
    {
        return prefab != null && poolDictionary.ContainsKey(prefab);
    }

    public GameObject Spawn(GameObject prefab, Vector3 position, Quaternion rotation)
    {
        if (prefab == null)
        {
            Debug.LogWarning("[ObjectPooler] Spawn prefab null");
            return null;
        }

        if (!poolDictionary.ContainsKey(prefab) || poolDictionary[prefab].Count == 0)
        {
            Debug.LogWarning($"[ObjectPooler] Nessun pool trovato per {prefab.name}");
            return null;
        }

        var obj = poolDictionary[prefab].Dequeue();
        obj.transform.position = position;
        obj.transform.rotation = rotation;
        obj.SetActive(true);

        poolDictionary[prefab].Enqueue(obj);
        return obj;
    }

    public T Spawn<T>(GameObject prefab, Vector3 position, Quaternion rotation) where T : Component
    {
        var go = Spawn(prefab, position, rotation);
        return go != null ? go.GetComponent<T>() : null;
    }

    public void ReturnToPool(GameObject obj)
    {
        if (obj == null) return;
        obj.SetActive(false);
    }

    public void ClearAllPools()
    {
        foreach (var kv in poolDictionary)
        {
            foreach (var go in kv.Value)
                if (go != null) Destroy(go);
        }
        poolDictionary.Clear();
    }

    public void ConfigurePoolsForClass(SOPlayerClass playerClass)
    {
        if (playerClass == null) return;

        foreach (var prefab in playerClass.poolPrefabs)
        {
            if (prefab == null) continue;
            if (!HasPrefab(prefab))
                AddToPool(prefab, playerClass.defaultPoolSize);
        }
    }
}


