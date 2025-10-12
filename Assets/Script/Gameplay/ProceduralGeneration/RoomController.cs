using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    public SORoom roomData;
    public EnemySpawner enemySpawner;

    [HideInInspector] public bool isPlayerInside;
    [HideInInspector] public bool roomCleared;

    private Vector3Int gridPos;
    private Dictionary<Vector3Int, RoomNode> allRooms;
    private List<GameObject> doors = new List<GameObject>();
    private List<GameObject> spawned = new List<GameObject>();

    private Dictionary<string, RoomController> connectedRooms = new Dictionary<string, RoomController>();

    public void Initialize(Vector3Int pos, Dictionary<Vector3Int, RoomNode> rooms)
    {
        gridPos = pos;
        allRooms = rooms;

        GenerateFloor();
        ConnectAdjacentRooms();
        GenerateWallsAndDoors();
    }

    private void GenerateFloor()
    {
        if (roomData.floorPrefabs == null || roomData.floorPrefabs.Count == 0) return;

        float startX = -((roomData.roomWidth - 1) / 2f) * (roomData.tileSize + roomData.floorSpacing);
        float startZ = -((roomData.roomLength - 1) / 2f) * (roomData.tileSize + roomData.floorSpacing);

        for (int x = 0; x < roomData.roomWidth; x++)
        {
            for (int z = 0; z < roomData.roomLength; z++)
            {
                GameObject prefab = roomData.floorPrefabs[Random.Range(0, roomData.floorPrefabs.Count)];
                if (!prefab) continue;
                Vector3 spawnPos = transform.position + new Vector3(startX + x * (roomData.tileSize + roomData.floorSpacing), roomData.floorHeight, startZ + z * (roomData.tileSize + roomData.floorSpacing));
                spawned.Add(Instantiate(prefab, spawnPos, Quaternion.identity, transform));
            }
        }
    }

    private void ConnectAdjacentRooms()
    {
        Vector3Int northPos = gridPos + Vector3Int.forward;
        Vector3Int southPos = gridPos + Vector3Int.back;
        Vector3Int eastPos = gridPos + Vector3Int.right;
        Vector3Int westPos = gridPos + Vector3Int.left;

        if (allRooms.ContainsKey(northPos)) connectedRooms["North"] = allRooms[northPos].controller;
        if (allRooms.ContainsKey(southPos)) connectedRooms["South"] = allRooms[southPos].controller;
        if (allRooms.ContainsKey(eastPos)) connectedRooms["East"] = allRooms[eastPos].controller;
        if (allRooms.ContainsKey(westPos)) connectedRooms["West"] = allRooms[westPos].controller;
    }

    private void GenerateWallsAndDoors()
    {
        GenerateWall(Vector3.forward, roomData.roomWidth, roomData.roomLength, "North");
        GenerateWall(Vector3.back, roomData.roomWidth, roomData.roomLength, "South");
        GenerateWall(Vector3.right, roomData.roomLength, roomData.roomWidth, "East");
        GenerateWall(Vector3.left, roomData.roomLength, roomData.roomWidth, "West");
    }

    private void GenerateWall(Vector3 dir, int segmentCount, int perpendicularCount, string direction)
    {
        float start = -((segmentCount - 1) / 2f) * (roomData.tileSize + roomData.wallSpacing);
        float offset = perpendicularCount * roomData.tileSize * 0.5f + roomData.wallOffset;

        for (int i = 0; i < segmentCount; i++)
        {
            bool isCenter = (i == segmentCount / 2);
            bool hasNeighbor = connectedRooms.ContainsKey(direction);

            GameObject prefab = (!isCenter || !hasNeighbor) ? roomData.wallPrefab : roomData.doorPrefab;
            if (!prefab) continue;

            float pos = start + i * (roomData.tileSize + roomData.wallSpacing);
            Vector3 posOffset = (dir == Vector3.forward || dir == Vector3.back) ? Vector3.right * pos : Vector3.forward * -pos;

            Vector3 spawnPos = transform.position + dir * offset + posOffset + Vector3.up * roomData.floorHeight;
            Quaternion rot = Quaternion.Euler(0, dir == Vector3.right ? 90f : dir == Vector3.left ? -90f : dir == Vector3.back ? 180f : 0f, 0);

            GameObject obj = Instantiate(prefab, spawnPos, rot, transform);
            if (isCenter && hasNeighbor)
            {
                doors.Add(obj);
                DoorTrigger dt = obj.AddComponent<DoorTrigger>();
                dt.Initialize(this, direction);
            }
        }
    }

    public void EnterDoor(string direction, GameObject player)
    {
        if (connectedRooms.TryGetValue(direction, out RoomController nextRoom))
        {
            Vector3 targetPos = nextRoom.transform.position;
            switch (direction)
            {
                case "North": targetPos += Vector3.back * 1.5f; break;
                case "South": targetPos += Vector3.forward * 1.5f; break;
                case "East": targetPos += Vector3.left * 1.5f; break;
                case "West": targetPos += Vector3.right * 1.5f; break;
            }
            player.transform.position = targetPos;
            player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        }
    }
}
