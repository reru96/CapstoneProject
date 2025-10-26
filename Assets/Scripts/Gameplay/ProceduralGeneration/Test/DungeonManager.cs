using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using Core;
using Gameplay;
using System;
using Random = UnityEngine.Random;

public class DungeonManager : MonoBehaviour
{

    public List<GameObject> roomPrefabs;
    public GameObject startRoomPrefab;
    public DungeonContentSpawner dungeonContentSpawner;

    public NavMeshSurface navMeshSurface;

    public int maxRooms = 10;
    private int roomCount = 0;

    private void Start()
    {
        SpawnNextRoom(Vector3.zero, RoomType.Start);
    }

    public void SpawnNextRoom(Vector3 spawnPosition, RoomType forcedType = RoomType.None)
    {
        if (roomCount >= maxRooms)
        {
            Debug.Log("Limite massimo di stanze raggiunto.");
            return;
        }

        GameObject prefab = forcedType == RoomType.Start
            ? startRoomPrefab
            : roomPrefabs[Random.Range(0, roomPrefabs.Count)];

        GameObject room = Instantiate(prefab, spawnPosition, Quaternion.identity);
        RoomMetaData metadata = room.GetComponent<RoomMetaData>();

        RoomType type = forcedType != RoomType.None ? forcedType : GetNextRoomType(roomCount);
        if (metadata != null)
            metadata.roomType = type;

        dungeonContentSpawner.SpawnContentInRoom(room, type);
        roomCount++;

        if (navMeshSurface != null)
            navMeshSurface.BuildNavMesh();
    }

    private RoomType GetNextRoomType(int index)
    {
        if (index == maxRooms - 1) return RoomType.Boss;
        if (index == maxRooms / 2) return RoomType.Rest;
        if (index == 1 || index == maxRooms - 2) return RoomType.Treasure;
        if (index >= maxRooms * 0.6f) return RoomType.Elite;
        return RoomType.Common;
    }
}