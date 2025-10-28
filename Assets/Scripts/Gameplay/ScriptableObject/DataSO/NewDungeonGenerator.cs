using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.AI.Navigation;
using UnityEngine;

public class NewDungeonGenerator : MonoBehaviour
{
    public DungeonConfig config;
    public Transform dungeonParent;
    public GameObject floor;
    public int gridWidth = 10;
    public int gridHeight = 10;
    public float cellSize = 10f;
    public float extraGap = 2f;
    [SerializeField] private NavMeshSurface surface;

    private List<PlacedRoom> placedRooms = new List<PlacedRoom>();

    void Awake()
    {
        surface = GetComponent<NavMeshSurface>();
    }

    void Start()
    {
        GenerateFloor();
        GenerateDungeon();
        surface.BuildNavMesh();
    }

    void GenerateFloor()
    {
        for (int x = 0; x < gridWidth; x++)
        {
            for (int z = 0; z < gridHeight; z++)
            {
                Vector3 pos = new Vector3(x * cellSize, 0, z * cellSize);
                Instantiate(floor, pos, Quaternion.identity, dungeonParent);
            }
        }
    }

    void GenerateDungeon()
    {
        var startRoom = config.rooms.Find(r => r.roomType == RoomType.Start);
        var bossRoom = config.rooms.Find(r => r.roomType == RoomType.Boss);
        var treasureRoom = config.rooms.Find(r => r.roomType == RoomType.Treasure);
        var restRoom = config.rooms.Find(r => r.roomType == RoomType.Rest);
        var eliteRoom = config.rooms.Find(r => r.roomType == RoomType.Elite);
        var normalRooms = config.rooms.FindAll(r => r.roomType == RoomType.Common);

        if (startRoom == null || bossRoom == null)
        {
            Debug.LogError("❌ Start o Boss room non presenti nel DungeonConfig");
            return;
        }

        PlaceRoomWithRandomPosition(bossRoom, 200, true);

        List<SORoom> otherRooms = new List<SORoom>();
        otherRooms.Add(startRoom);
        if (treasureRoom != null) otherRooms.Add(treasureRoom);
        if (restRoom != null) otherRooms.Add(restRoom);
        if (eliteRoom != null) otherRooms.Add(eliteRoom);

        int remaining = config.roomCount - placedRooms.Count;
        for (int i = 0; i < remaining; i++)
        {
            if (normalRooms.Count > 0)
                otherRooms.Add(normalRooms[Random.Range(0, normalRooms.Count)]);
        }

        foreach (var room in otherRooms)
        {
            PlaceRoomWithRandomPosition(room, 200);
        }

        Debug.Log("✅ Dungeon generato senza sovrapposizioni!");
    }

    void PlaceRoomWithRandomPosition(SORoom room, int maxAttempts, bool isBoss = false)
    {
        bool placed = false;
        for (int attempt = 0; attempt < maxAttempts; attempt++)
        {
            Vector3 randomPos = new Vector3(
                Random.Range(room.roomWorldSize.x / 2, gridWidth * cellSize - room.roomWorldSize.x / 2),
                0,
                Random.Range(room.roomWorldSize.z / 2, gridHeight * cellSize - room.roomWorldSize.z / 2)
            );

            if (CanPlaceRoom(randomPos, room.roomWorldSize))
            {
                PlaceRoom(room, randomPos);
                placed = true;
                break;
            }
        }

        if (!placed)
        {
            if (isBoss)
                Debug.LogError($"Impossibile piazzare la Boss Room dopo {maxAttempts} tentativi.");
            else
                Debug.LogWarning($"Impossibile piazzare {room.roomName} dopo {maxAttempts} tentativi.");
        }
    }

    bool CanPlaceRoom(Vector3 pos, Vector3 size)
    {
        if (pos.x - size.x / 2 < 0 || pos.z - size.z / 2 < 0 ||
            pos.x + size.x / 2 > gridWidth * cellSize || pos.z + size.z / 2 > gridHeight * cellSize)
            return false;

        foreach (var r in placedRooms)
        {
            if (RectOverlap(pos, size, r.position, r.size))
                return false;
        }

        return true;
    }

    void PlaceRoom(SORoom room, Vector3 pos)
    {
        Instantiate(room.roomPrefab, pos, Quaternion.identity, dungeonParent);

        placedRooms.Add(new PlacedRoom
        {
            position = pos,
            size = room.roomWorldSize
        });
    }

    bool RectOverlap(Vector3 pos1, Vector3 size1, Vector3 pos2, Vector3 size2)
    {
        return (Mathf.Abs(pos1.x - pos2.x) * 2 < (size1.x + size2.x + extraGap)) &&
               (Mathf.Abs(pos1.z - pos2.z) * 2 < (size1.z + size2.z + extraGap));
    }

    class PlacedRoom
    {
        public Vector3 position;
        public Vector3 size;
    }
}
