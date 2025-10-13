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

    private PlayerSpawnManager spawnManager;
    private ClassSelectionManager classManager;

    private void Start()
    {
        spawnManager = CoreSystem.Instance.Container.Resolve<PlayerSpawnManager>();
        classManager = CoreSystem.Instance.Container.Resolve<ClassSelectionManager>();

        GenerateDungeon();
    }

    public void GenerateDungeon()
    {

        generator.GenerateDungeon();
        spawner.SpawnRooms();

        if (navMeshSurface != null)
            navMeshSurface.BuildNavMesh();

        CreateRespawnPointInStartRoom();

    }

    private void CreateRespawnPointInStartRoom()
    {
        if (generator == null || spawner == null) return;

        Vector3Int startPos = generator.startPos;
        if (!spawner.spawnedRooms.TryGetValue(startPos, out GameObject startRoom))
            startRoom = spawner.spawnedRooms.Values.FirstOrDefault();

        if (startRoom != null)
        {

            GameObject respawn = GameObject.FindGameObjectWithTag("RespawnPoint");
            if (respawn == null)
            {
                respawn = new GameObject("RespawnPoint");
                respawn.tag = "RespawnPoint";
            }

            respawn.transform.position = startRoom.transform.position + Vector3.up * 1.5f;
        }
        else
        {
            Debug.LogWarning("[DungeonManager] StartRoom non trovata, spawn point fallback a zero.");
            GameObject respawn = GameObject.FindGameObjectWithTag("RespawnPoint");
            if (respawn == null)
            {
                respawn = new GameObject("RespawnPoint");
                respawn.tag = "RespawnPoint";
            }
            respawn.transform.position = Vector3.zero;
        }
    }
}
