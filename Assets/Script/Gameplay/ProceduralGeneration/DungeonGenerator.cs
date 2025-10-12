using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.AI.Navigation;
using UnityEngine;

public class DungeonGenerator : MonoBehaviour
{
    public GameObject roomControllerPrefab;
    public SORoom defaultRoomData;
    public SORoom bossRoomData;
    public SORoom restRoomData;
    public int walkLength = 20;
    public Vector3Int startPos = Vector3Int.zero;
    public int gridLimit = 1000;

    public Dictionary<Vector3Int, RoomNode> dungeonRooms = new Dictionary<Vector3Int, RoomNode>();

    void Start()
    {
        GenerateDungeon();
    }

    public void GenerateDungeon()
    {
        dungeonRooms.Clear();

        AddRoomAt(startPos, defaultRoomData, true);

        Vector3Int currentPos = startPos;

        for (int i = 0; i < walkLength; i++)
        {
            Vector3Int nextPos = currentPos + GetRandomDirection();

            if (Mathf.Abs(nextPos.x) > gridLimit || Mathf.Abs(nextPos.z) > gridLimit)
                continue;

            SORoom data = defaultRoomData;
            RoomBounds tempBounds = new RoomBounds(
                nextPos - new Vector3Int(data.roomWidth / 2, 0, data.roomLength / 2),
                nextPos + new Vector3Int((data.roomWidth - 1) / 2, 0, (data.roomLength - 1) / 2)
            );

            if (IsOverlapping(tempBounds))
                continue;

            AddRoomAt(nextPos, data);
            currentPos = nextPos;
        }

        AssignSpecialRooms();
    }

    private void AddRoomAt(Vector3Int pos, SORoom data, bool isStart = false)
    {
        GameObject newRoom = Instantiate(roomControllerPrefab, new Vector3(pos.x, 0, pos.z), Quaternion.identity, transform);
        RoomController rc = newRoom.GetComponent<RoomController>();
        if (rc != null)
        {
            rc.roomData = data;
            rc.Initialize(pos, dungeonRooms);
        }

        RoomNode node = new RoomNode
        {
            gridPos = pos,
            isStart = isStart,
            controller = rc,
            roomBounds = new RoomBounds
            (
                pos - new Vector3Int(data.roomWidth / 2, 0, data.roomLength / 2),
                pos + new Vector3Int((data.roomWidth - 1) / 2, 0, (data.roomLength - 1) / 2)
            )
        };

        dungeonRooms[pos] = node;
    }

    private bool IsOverlapping(RoomBounds bounds)
    {
        foreach (var node in dungeonRooms.Values)
        {
            if (node.roomBounds.Intersects(bounds))
                return true;
        }
        return false;
    }

    private void AssignSpecialRooms()
    {
        if (dungeonRooms.Count <= 1) return;

        Vector3Int bossPos = GetFarthestRoom(startPos);
        if (dungeonRooms.ContainsKey(bossPos))
        {
            dungeonRooms[bossPos].isBoss = true;
            dungeonRooms[bossPos].controller.roomData = bossRoomData;
            dungeonRooms[bossPos].controller.Initialize(bossPos, dungeonRooms);
        }

        Vector3Int restPos = GetRoomByPathIndex(startPos, dungeonRooms.Count / 2);
        if (dungeonRooms.ContainsKey(restPos))
        {
            dungeonRooms[restPos].isRest = true;
            dungeonRooms[restPos].controller.roomData = restRoomData;
            dungeonRooms[restPos].controller.Initialize(restPos, dungeonRooms);
        }
    }

    private Vector3Int GetRandomDirection()
    {
        switch (Random.Range(0, 4))
        {
            case 0: return Vector3Int.right;
            case 1: return Vector3Int.left;
            case 2: return Vector3Int.forward;
            case 3: return Vector3Int.back;
        }
        return Vector3Int.zero;
    }

    private Vector3Int GetFarthestRoom(Vector3Int from)
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

    private Vector3Int GetRoomByPathIndex(Vector3Int start, int index)
    {
        List<Vector3Int> sortedRooms = new List<Vector3Int>(dungeonRooms.Keys);
        sortedRooms.Sort((a, b) => Vector3Int.Distance(start, a).CompareTo(Vector3Int.Distance(start, b)));
        index = Mathf.Clamp(index, 0, sortedRooms.Count - 1);
        return sortedRooms[index];
    }
}
