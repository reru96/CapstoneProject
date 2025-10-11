using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public DungeonGenerator dungeonGenerator;
    public GameObject defaultRoomPrefab;
    public GameObject bossRoomPrefab;
    public GameObject restRoomPrefab;
    public float roomSpacing = 25f;
    public NavMeshSurface navMeshSurface;

    [HideInInspector]
    public Dictionary<Vector3Int, GameObject> spawnedRooms = new Dictionary<Vector3Int, GameObject>();

    public void SpawnRooms()
    {
        ClearOldRooms();

        foreach (var room in dungeonGenerator.dungeonRooms.Values)
        {
            Vector3 pos = new Vector3(
                room.gridPos.x * roomSpacing,
                0,
                room.gridPos.z * roomSpacing
            );

            GameObject prefab = GetRoomPrefab(room);
            GameObject newRoom = Instantiate(prefab, pos, Quaternion.identity, transform);

            RoomController rc = newRoom.GetComponent<RoomController>();
            if (rc != null)
                rc.Initialize(room.gridPos, dungeonGenerator.dungeonRooms);

            spawnedRooms.Add(room.gridPos, newRoom);
        }

        if (navMeshSurface != null)
            navMeshSurface.BuildNavMesh();
    }

    void ClearOldRooms()
    {
        foreach (Transform child in transform)
            DestroyImmediate(child.gameObject);
        spawnedRooms.Clear();
    }

    GameObject GetRoomPrefab(RoomNode room)
    {
        if (room.isBoss) return bossRoomPrefab;
        if (room.isRest) return restRoomPrefab;
        return defaultRoomPrefab;
    }
}
