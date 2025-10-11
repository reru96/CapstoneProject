using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{

    public int walkLength = 20;

    public Vector3Int startPos = Vector3Int.zero;

    public int gridLimit = 10;

    [HideInInspector]
    public Dictionary<Vector3Int, RoomNode> dungeonRooms = new Dictionary<Vector3Int, RoomNode>();

    void Start()
    {
        GenerateDungeon();
    }

    public void GenerateDungeon()
    {
        dungeonRooms.Clear();

        Vector3Int currentPos = startPos;
        dungeonRooms[currentPos] = new RoomNode { gridPos = currentPos, isStart = true };

  
        for (int i = 0; i < walkLength; i++)
        {
            Vector3Int nextPos = currentPos + GetRandomDirection();

          
            if (Mathf.Abs(nextPos.x) > gridLimit || Mathf.Abs(nextPos.z) > gridLimit)
                continue;

            if (!dungeonRooms.ContainsKey(nextPos))
                dungeonRooms[nextPos] = new RoomNode { gridPos = nextPos };

            currentPos = nextPos;
        }


        AssignSpecialRooms();
    }

    void AssignSpecialRooms()
    {
        if (dungeonRooms.Count <= 1) return;

        Vector3Int bossPos = GetFarthestRoom(startPos);
        if (dungeonRooms.ContainsKey(bossPos))
            dungeonRooms[bossPos].isBoss = true;

        Vector3Int restPos = GetMidRoomByPath(startPos, bossPos);
        if (dungeonRooms.ContainsKey(restPos))
            dungeonRooms[restPos].isRest = true;
    }

    Vector3Int GetRandomDirection()
    {
        switch (Random.Range(0, 4))
        {
            case 0: return new Vector3Int(1, 0, 0);
            case 1: return new Vector3Int(-1, 0, 0);
            case 2: return new Vector3Int(0, 0, 1);
            case 3: return new Vector3Int(0, 0, -1);
            default: return Vector3Int.zero;
        }
    }

    Vector3Int GetFarthestRoom(Vector3Int from)
    {
        Vector3Int farthest = from;
        float maxDist = 0f;

        foreach (var kv in dungeonRooms)
        {
            float dist = Vector3Int.Distance(from, kv.Key);
            if (dist > maxDist)
            {
                maxDist = dist;
                farthest = kv.Key;
            }
        }

        return farthest;
    }

    Vector3Int GetMidRoomByPath(Vector3Int start, Vector3Int end)
    {
        List<Vector3Int> sortedRooms = dungeonRooms.Keys
            .OrderBy(pos => Vector3Int.Distance(start, pos))
            .ToList();

        if (sortedRooms.Count == 0)
            return start;

        int midIndex = sortedRooms.Count / 2;
        return sortedRooms[midIndex];
    }
}
