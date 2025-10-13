using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public int roomCount = 8;
    public int maxBranching = 3;
    public Vector3Int startPos = Vector3Int.zero;

    [HideInInspector]
    public Dictionary<Vector3Int, RoomNode> dungeonRooms = new Dictionary<Vector3Int, RoomNode>();

    public void GenerateDungeon()
    {
        dungeonRooms.Clear();

        Vector3Int currentPos = Vector3Int.zero;
        dungeonRooms[currentPos] = new RoomNode(currentPos);

        for (int i = 1; i < roomCount; i++)
        {
            Vector3Int newPos = GetNextRoomPos(currentPos);
            if (dungeonRooms.ContainsKey(newPos))
            {
                currentPos = GetRandomExistingRoomPos();
                i--;
                continue;
            }

            dungeonRooms[newPos] = new RoomNode(newPos);
            currentPos = newPos;
        }

        if (dungeonRooms.Count > 0)
        {
            var lastRoom = new List<Vector3Int>(dungeonRooms.Keys)[dungeonRooms.Count - 1];
            dungeonRooms[lastRoom].isBoss = true;
        }

        LinkNeighbors();
    }


    Vector3Int GetNextRoomPos(Vector3Int current)
    {
        Vector3Int[] dirs =
        {
            Vector3Int.forward,
            Vector3Int.back,
            Vector3Int.left,
            Vector3Int.right
        };

        Vector3Int dir = dirs[Random.Range(0, dirs.Length)];
        return current + dir;
    }


    Vector3Int GetRandomExistingRoomPos()
    {
        var keys = new List<Vector3Int>(dungeonRooms.Keys);
        return keys[Random.Range(0, keys.Count)];
    }


    void LinkNeighbors()
    {
        foreach (var kvp in dungeonRooms)
        {
            Vector3Int pos = kvp.Key;
            RoomNode room = kvp.Value;

            if (dungeonRooms.TryGetValue(pos + Vector3Int.forward, out RoomNode north))
            {
                room.AddNeighbor(Direction.North, north);
                north.AddNeighbor(Direction.South, room);
            }

            if (dungeonRooms.TryGetValue(pos + Vector3Int.back, out RoomNode south))
            {
                room.AddNeighbor(Direction.South, south);
                south.AddNeighbor(Direction.North, room);
            }

            if (dungeonRooms.TryGetValue(pos + Vector3Int.right, out RoomNode east))
            {
                room.AddNeighbor(Direction.East, east);
                east.AddNeighbor(Direction.West, room);
            }

            if (dungeonRooms.TryGetValue(pos + Vector3Int.left, out RoomNode west))
            {
                room.AddNeighbor(Direction.West, west);
                west.AddNeighbor(Direction.East, room);
            }
        }
    }

}
