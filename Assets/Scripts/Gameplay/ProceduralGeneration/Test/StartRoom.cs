using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using Unity.AI.Navigation;
using UnityEngine;

public class StartRoom : MonoBehaviour
{
    public Vector2Int startRoomSize = new Vector2Int(10, 10);
    public GameObject floorPrefab;
    public GameObject wallPrefab;
    public NavMeshSurface navMeshSurface;


    public DungeonCreator dungeonCreator;

    private Transform playerSpawnPoint;
    private GameObject startRoom;

    void Start()
    {
        CreateStartRoom();

        if (navMeshSurface != null) navMeshSurface.BuildNavMesh();

     
        var spawnManager = ServiceLocator.Get<PlayerSpawnManager>();
        if (spawnManager != null)
            spawnManager.SetRespawnPoint(playerSpawnPoint);

        if (dungeonCreator != null)
            dungeonCreator.OnDungeonReady += ConnectStartRoomToDungeon;
        dungeonCreator?.CreateDungeon();
    }

    void CreateStartRoom()
    {
        startRoom = new GameObject("StartRoom");
        startRoom.transform.parent = transform;

    
        GameObject floor = Instantiate(floorPrefab, new Vector3(startRoomSize.x / 2f, 0, startRoomSize.y / 2f), Quaternion.identity, startRoom.transform);
        floor.transform.localScale = new Vector3(startRoomSize.x, 1, startRoomSize.y);

        List<Transform> walls = new List<Transform>();
        if (wallPrefab != null)
        {
            for (int x = 0; x <= startRoomSize.x; x++)
            {
                walls.Add(Instantiate(wallPrefab, new Vector3(x, 0, 0), Quaternion.identity, startRoom.transform).transform);
                walls.Add(Instantiate(wallPrefab, new Vector3(x, 0, startRoomSize.y), Quaternion.identity, startRoom.transform).transform);
            }
            for (int z = 1; z < startRoomSize.y; z++)
            {
                walls.Add(Instantiate(wallPrefab, new Vector3(0, 0, z), Quaternion.identity, startRoom.transform).transform);
                walls.Add(Instantiate(wallPrefab, new Vector3(startRoomSize.x, 0, z), Quaternion.identity, startRoom.transform).transform);
            }
        }

        GameObject sp = new GameObject("PlayerSpawnPoint");
        sp.transform.position = new Vector3(startRoomSize.x / 2f, 1f, startRoomSize.y / 2f);
        sp.transform.parent = startRoom.transform;
        playerSpawnPoint = sp.transform;
    }

    void ConnectStartRoomToDungeon()
    {
        if (dungeonCreator.listOfRooms == null || dungeonCreator.listOfRooms.Count == 0) return;

        var firstRoom = dungeonCreator.listOfRooms[0];

        Transform closestWall = null;
        float minDist = float.MaxValue;
        foreach (Transform wall in startRoom.transform)
        {
            float dist = Vector3.Distance(wall.position, firstRoom.CenterPosition());
            if (dist < minDist)
            {
                minDist = dist;
                closestWall = wall;
            }
        }

        if (closestWall != null) Destroy(closestWall.gameObject);

        if (navMeshSurface != null) navMeshSurface.BuildNavMesh();
    }
}
