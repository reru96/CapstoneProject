using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using System.Linq;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
    public DungeonGenerator generator;
    public RoomGenerator spawner;
    public NavMeshSurface navMeshSurface;
    public GameObject playerPrefab;

    private GameObject playerInstance;

    void Start()
    {
        GenerateDungeon();
    }

    public void GenerateDungeon()
    {
        generator.GenerateDungeon();
        spawner.SpawnRooms();

        if (navMeshSurface != null)
            navMeshSurface.BuildNavMesh();

        SpawnPlayer();
    }

    void SpawnPlayer()
    {
        if (playerPrefab == null) return;

        Vector3Int startPos = generator.startPos;

        if (!spawner.spawnedRooms.TryGetValue(startPos, out GameObject startRoom))
        {
            startRoom = spawner.spawnedRooms.Values.FirstOrDefault();
        }

        if (startRoom != null)
        {
            Vector3 spawnPos = startRoom.transform.position + Vector3.up * 1.5f;
            if (playerInstance != null) Destroy(playerInstance);
            playerInstance = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        }
    }
}
