using System;
using System.Collections;
using System.Collections.Generic;
using Codice.Client.Common;
using Unity.AI.Navigation;
using UnityEngine;
using Random = UnityEngine.Random;


public class RoomController : MonoBehaviour
{
    public SORoom roomData;
    public Vector3Int gridPos;
    public RoomNode node;

    private float tileSize;
    private float offset;

    private Dictionary<Direction, Vector3> dirToVector;

    void Awake()
    {
        dirToVector = new Dictionary<Direction, Vector3>
        {
            { Direction.North, Vector3.forward },
            { Direction.South, Vector3.back },
            { Direction.East,  Vector3.right },
            { Direction.West,  Vector3.left }
        };
    }

    public void Initialize(Vector3Int position, Dictionary<Vector3Int, RoomNode> allRooms)
    {
        gridPos = position;
        node = allRooms[position];

        tileSize = roomData.tileSize;
        offset = roomData.offset;

        GenerateFloor();
        GenerateWallsAndDoors(allRooms);
    }

    public void GenerateFloor()
    {
        int width = roomData.roomWidth;
        int length = roomData.roomLength;
        float spacing = roomData.floorSpacing;

        float startX = -((width - 1) * (tileSize + spacing) / 2f);
        float startZ = -((length - 1) * (tileSize + spacing) / 2f);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                Vector3 pos = new Vector3(
                    startX + x * (tileSize + spacing),
                    0,
                    startZ + z * (tileSize + spacing)
                );

                GameObject prefab = roomData.floorPrefabs[Random.Range(0, roomData.floorPrefabs.Count)];
                Instantiate(prefab, transform.position + pos, Quaternion.identity, transform);
            }
        }
    }

    public void GenerateWallsAndDoors(Dictionary<Vector3Int, RoomNode> allRooms)
    {
        int width = roomData.roomWidth;
        int length = roomData.roomLength;

        float startX = -((width - 1) * tileSize / 2f);
        float startZ = -((length - 1) * tileSize / 2f);

        foreach (Direction dir in Enum.GetValues(typeof(Direction)))
        {
            Vector3Int neighborPos = gridPos + DirectionToVectorInt(dir);
            bool hasNeighbor = allRooms.ContainsKey(neighborPos);

            RoomNode neighbor = hasNeighbor ? allRooms[neighborPos] : null;
            bool isConnected = (neighbor != null);

            int segmentCount = (dir == Direction.North || dir == Direction.South) ? width : length;

            for (int i = 0; i < segmentCount; i++)
            {
                bool isCenter = (i == segmentCount / 2);
                GameObject prefabToSpawn = (isConnected && isCenter) ? roomData.doorPrefab : roomData.wallPrefab;
                if (prefabToSpawn == null) continue;

                Vector3 localPos = Vector3.zero;
                Quaternion rot = Quaternion.identity;

                switch (dir)
                {
                    case Direction.North:
                        localPos = new Vector3(startX + i * tileSize, 0, startZ + (length - 1) * tileSize + roomData.wallOffsetZ);
                        rot = Quaternion.identity;
                        break;
                    case Direction.South:
                        localPos = new Vector3(startX + i * tileSize, 0, startZ - roomData.wallOffsetZ);
                        rot = Quaternion.Euler(0, 180, 0);
                        break;
                    case Direction.East:
                        localPos = new Vector3(startX + (width - 1) * tileSize + roomData.wallOffsetX, 0, startZ + i * tileSize);
                        rot = Quaternion.Euler(0, 90, 0);
                        break;
                    case Direction.West:
                        localPos = new Vector3(startX - roomData.wallOffsetX, 0, startZ + i * tileSize);
                        rot = Quaternion.Euler(0, -90, 0);
                        break;
                }

                GameObject wallOrDoor = Instantiate(prefabToSpawn, transform.position + localPos, rot, transform);

                if (prefabToSpawn == roomData.doorPrefab && neighbor != null && neighbor.controller != null)
                {
                    LinkDoor(wallOrDoor, neighbor.controller, Opposite(dir));
                }
            }
        }
    }

    void LinkDoor(GameObject door, RoomController neighborController, Direction oppositeDir)
    {
        DoorLinker linker = door.GetComponent<DoorLinker>();
        if (linker != null)
        {
            linker.targetRoom = neighborController;
            linker.targetDirection = oppositeDir;
        }

        CreateNavMeshLink(door.transform, neighborController, oppositeDir);
    }

    void CreateNavMeshLink(Transform doorTransform, RoomController neighborController, Direction oppositeDir)
    {
        if (neighborController == null) return;

        Vector3 startPos = doorTransform.position;
        Vector3 endPos = neighborController.transform.position;

        Vector3 dirOffset = Vector3.zero;
        switch (oppositeDir)
        {
            case Direction.North: dirOffset = Vector3.forward * 2f; break;
            case Direction.South: dirOffset = Vector3.back * 2f; break;
            case Direction.East: dirOffset = Vector3.right * 2f; break;
            case Direction.West: dirOffset = Vector3.left * 2f; break;
        }
        endPos += dirOffset;

        NavMeshLink link = doorTransform.GetComponent<NavMeshLink>();
        if (link == null) link = doorTransform.gameObject.AddComponent<NavMeshLink>();

        link.startPoint = doorTransform.InverseTransformPoint(startPos);
        link.endPoint = doorTransform.InverseTransformPoint(endPos);
        link.width = 2f;
        link.costModifier = -1;
        link.bidirectional = true;
        link.area = 0;
    }

    Direction Opposite(Direction dir)
    {
        return dir switch
        {
            Direction.North => Direction.South,
            Direction.South => Direction.North,
            Direction.East => Direction.West,
            Direction.West => Direction.East,
            _ => Direction.North
        };
    }

    Vector3Int DirectionToVectorInt(Direction dir)
    {
        return dir switch
        {
            Direction.North => new Vector3Int(0, 0, 1),
            Direction.South => new Vector3Int(0, 0, -1),
            Direction.East => new Vector3Int(1, 0, 0),
            Direction.West => new Vector3Int(-1, 0, 0),
            _ => Vector3Int.zero
        };
    }
}