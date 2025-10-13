using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class RoomController : MonoBehaviour
{

    public SORoom roomData;

    public Vector3Int gridPos;
    public RoomNode node;

    private float tileSize = 4f;
    private float offset = 0f;

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
        GenerateWallsAndDoors();
    }

    void GenerateFloor()
    {
        int width = roomData.roomWidth;
        int length = roomData.roomLength;
        float offset = roomData.floorSpacing; 

        float startX = -((width - 1) * (tileSize + offset) / 2f);
        float startZ = -((length - 1) * (tileSize + offset) / 2f);

        for (int x = 0; x < width; x++)
        {
            for (int z = 0; z < length; z++)
            {
                Vector3 pos = new Vector3(
                    startX + x * (tileSize + offset),
                    0,
                    startZ + z * (tileSize + offset)
                );

                GameObject prefab = roomData.floorPrefabs[Random.Range(0, roomData.floorPrefabs.Count)];
                Instantiate(prefab, transform.position + pos, Quaternion.identity, transform);
            }
        }
    }

    void GenerateWallsAndDoors()
    {
        int width = roomData.roomWidth;
        int length = roomData.roomLength;
        float tileSize = roomData.tileSize;

        float startX = -((width - 1) * tileSize / 2f);
        float startZ = -((length - 1) * tileSize / 2f);

        foreach (Direction dir in System.Enum.GetValues(typeof(Direction)))
        {
            RoomNode neighbor = node.GetNeighbor(dir);
            bool hasNeighbor = (neighbor != null);

            int segmentCount = (dir == Direction.North || dir == Direction.South) ? width : length;

            for (int i = 0; i < segmentCount; i++)
            {
                bool isCenter = (i == segmentCount / 2);
                GameObject prefabToSpawn = (hasNeighbor && isCenter) ? roomData.doorPrefab : roomData.wallPrefab;
                if (prefabToSpawn == null) continue;

                Vector3 localPos = Vector3.zero;
                Quaternion rot = Quaternion.identity;

                switch (dir)
                {
                    case Direction.North:
                        localPos = new Vector3( startX + i * tileSize, 0, startZ + (length - 1) * tileSize + roomData.wallOffsetZ);
                        rot = Quaternion.identity;
                        break;

                    case Direction.South:
                        localPos = new Vector3( startX + i * tileSize, 0, startZ - roomData.wallOffsetZ);
                        rot = Quaternion.Euler(0, 180, 0);
                        break;

                    case Direction.East:
                        localPos = new Vector3(
                            startX + (width - 1) * tileSize + roomData.wallOffsetX, 0, startZ + i * tileSize);
                        rot = Quaternion.Euler(0, 90, 0);
                        break;

                    case Direction.West:
                        localPos = new Vector3(startX - roomData.wallOffsetX, 0, startZ + i * tileSize);
                        rot = Quaternion.Euler(0, -90, 0);
                        break;
                }

                GameObject wallOrDoor = Instantiate(prefabToSpawn, transform.position + localPos, rot, transform);

                if (prefabToSpawn == roomData.doorPrefab && hasNeighbor && neighbor.controller != null)
                    LinkDoors(wallOrDoor, neighbor.controller, Opposite(dir));
            }
        }
    }

    void LinkDoors(GameObject door, RoomController neighborController, Direction oppositeDir)
    {

        DoorLinker linker = door.GetComponent<DoorLinker>();
        if (linker != null)
        {
            linker.targetRoom = neighborController;
            linker.targetDirection = oppositeDir;
        }
    }

    Direction Opposite(Direction dir)
    {
        switch (dir)
        {
            case Direction.North: return Direction.South;
            case Direction.South: return Direction.North;
            case Direction.East: return Direction.West;
            case Direction.West: return Direction.East;
        }
        return Direction.North;
    }
}
