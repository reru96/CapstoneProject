using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class RoomGenerator : MonoBehaviour
{
    public DungeonGenerator dungeonGenerator;
    public GameObject roomControllerPrefab;
    public SORoom defaultRoomData, bossRoomData, restRoomData;
    public NavMeshSurface navMeshSurface;
    public float roomSpacing = 25f;

    [HideInInspector] public Dictionary<Vector3Int, GameObject> spawnedRooms = new Dictionary<Vector3Int, GameObject>();

    public void SpawnRooms()
    {
        foreach (var room in dungeonGenerator.dungeonRooms.Values)
        {
            Vector3 pos = new Vector3(room.gridPos.x * roomSpacing, 0, room.gridPos.z * roomSpacing);
            GameObject newRoom = Instantiate(roomControllerPrefab, pos, Quaternion.identity, transform);

            RoomController rc = newRoom.GetComponent<RoomController>();
            if (rc != null)
            {
                rc.roomData = GetRoomData(room);
                rc.Initialize(room.gridPos, dungeonGenerator.dungeonRooms);
                room.controller = rc;
            }

            spawnedRooms[room.gridPos] = newRoom;
        }

        if (navMeshSurface != null)
            navMeshSurface.BuildNavMesh();
    }

    SORoom GetRoomData(RoomNode room)
    {
        if (room.isBoss) return bossRoomData;
        if (room.isRest) return restRoomData;
        return defaultRoomData;
    }
}
