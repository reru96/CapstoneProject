using System.Collections;
using System.Collections.Generic;
using Core;
using Unity.AI.Navigation;
using Gameplay;
using UnityEngine;

public class RoomPoolManager : MonoBehaviour
{
    [Header("Pool Settings")]
    public GameObject roomPrefab;
    public int initialPoolSize = 10;

    private readonly List<Transform> pool = new List<Transform>();
    private readonly List<Transform> activeRooms = new List<Transform>();

    private void Awake()
    {
        for (int i = 0; i < initialPoolSize; i++)
        {
            Transform room = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity, transform).transform;
            room.gameObject.SetActive(false);
            pool.Add(room);
        }
    }

    public Transform SpawnRoom()
    {
        Transform room;

        if (pool.Count > 0)
        {
            room = pool[0];
            pool.RemoveAt(0);
        }
        else
        {
            room = Instantiate(roomPrefab, Vector3.zero, Quaternion.identity, transform).transform;
        }

        room.gameObject.SetActive(true);
        activeRooms.Add(room);
        return room;
    }

    public void DespawnRoom(Transform room)
    {
        if (!activeRooms.Contains(room)) return;

        room.gameObject.SetActive(false);
        room.position = Vector3.zero;
        room.rotation = Quaternion.identity;

        activeRooms.Remove(room);
        pool.Add(room);
    }

    public void ResetAll()
    {
        for (int i = activeRooms.Count - 1; i >= 0; i--)
            DespawnRoom(activeRooms[i]);
    }

    public Transform GetRoom(int index)
    {
        if (index < 0 || index >= activeRooms.Count) return null;
        return activeRooms[index];
    }
}
