using System.Collections.Generic;
using UnityEngine;
using Core;
using Gameplay;

[System.Serializable]
public class RoomTypePrefabPair
{
    public string roomType; 
    public GameObject prefab;
    public int initialPoolSize = 5;
}

public class RoomPoolManager : MonoBehaviour
{
    [Header("Room Pool Settings")]
    public List<RoomTypePrefabPair> roomPrefabs = new List<RoomTypePrefabPair>();

    private Dictionary<string, Queue<GameObject>> poolDictionary = new Dictionary<string, Queue<GameObject>>();

    private void Awake()
    {
        InitializePools();
    }

    private void InitializePools()
    {
        foreach (var pair in roomPrefabs)
        {
            if (pair.prefab == null || string.IsNullOrEmpty(pair.roomType))
                continue;

            var queue = new Queue<GameObject>();

            for (int i = 0; i < pair.initialPoolSize; i++)
            {
                var obj = Instantiate(pair.prefab);
                obj.name = $"{pair.roomType}_Room_{i}";
                obj.SetActive(false);
                queue.Enqueue(obj);
            }

            poolDictionary[pair.roomType] = queue;
        }
    }

    public GameObject GetFromPool(string roomType)
    {
        if (!poolDictionary.ContainsKey(roomType))
        {
            Debug.LogWarning($"[RoomPoolManager] Room type '{roomType}' not found!");
            return null;
        }

        var pool = poolDictionary[roomType];

        GameObject obj;
        if (pool.Count > 0)
            obj = pool.Dequeue();
        else
            obj = Instantiate(roomPrefabs.Find(p => p.roomType == roomType).prefab);

        obj.SetActive(true);
        return obj;
    }

    public void ReturnToPool(string roomType, GameObject room)
    {
        if (!poolDictionary.ContainsKey(roomType))
        {
            Destroy(room);
            return;
        }

        room.SetActive(false);
        poolDictionary[roomType].Enqueue(room);
    }

    public void ResetAll()
    {
        foreach (var queue in poolDictionary.Values)
        {
            foreach (var obj in queue)
                obj.SetActive(false);
        }
    }
}
