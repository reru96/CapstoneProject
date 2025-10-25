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
    [Header("Dungeon Settings")]
    public int maxRooms = 10;
    public DungeonContentSpawner contentSpawner;
    public RoomPoolManager poolManager;

    [Header("NavMesh")]
    public NavMeshSurface navMeshSurface;

    [Header("Generated Data")]
    public List<Room> generatedRooms = new List<Room>();
    public List<GameObject> activeRooms = new List<GameObject>();


    private void Start()
    {
        contentSpawner = GetComponent<DungeonContentSpawner>();
        SpawnStartRoom();
       

        if (navMeshSurface != null)
            navMeshSurface.BuildNavMesh();
    }

    private void SpawnStartRoom()
    {
        Room startRoom = new Room(RoomType.Start, new Vector2Int(0, 0), new Vector2Int(10, 10));
        GameObject roomObj = SpawnRoom(startRoom, RoomType.Start);
        generatedRooms.Add(startRoom);
        contentSpawner.SpawnContentInRoom(roomObj, RoomType.Start); 
        SpawnConnectedRooms(startRoom);
    }

    public void SpawnConnectedRooms(Room previousRoom)
    {
        if (generatedRooms.Count >= maxRooms) return;

        int numNewRooms = Random.Range(1, 3);
        for (int i = 0; i < numNewRooms; i++)
        {
            RoomType type = GetNextRoomType(generatedRooms.Count);

            Vector2Int bl = previousRoom.topRight + new Vector2Int(Random.Range(2, 5), 0);
            Vector2Int tr = bl + new Vector2Int(10, 10);

            Room corridor = new Room(RoomType.Corridor, previousRoom.topRight, bl);
            SpawnRoom(corridor, RoomType.Corridor);
            generatedRooms.Add(corridor);

            Room newRoom = new Room(type, bl, tr);
            SpawnRoom(newRoom, type);
            generatedRooms.Add(newRoom);
        }

        if (navMeshSurface != null)
            navMeshSurface.BuildNavMesh();
    }

    private GameObject SpawnRoom(Room room, RoomType type)
    {
        GameObject obj = poolManager.GetFromPool(type);
        obj.transform.position = room.GetCenter();
        obj.transform.rotation = Quaternion.identity;
        obj.transform.SetParent(transform);

        RoomTrigger trigger = obj.GetComponent<RoomTrigger>();
        if (trigger != null)
        {
            trigger.dungeonManager = this;
            trigger.roomData = room;
        }

        activeRooms.Add(obj); 
        return obj;
    }

    private RoomType GetNextRoomType(int index)
    {
        if (index == maxRooms - 1) return RoomType.Boss;
        if (index >= maxRooms * 0.6f) return RoomType.Elite;
        if (index == maxRooms / 2) return RoomType.Rest;
        return RoomType.Common;
    }
}
