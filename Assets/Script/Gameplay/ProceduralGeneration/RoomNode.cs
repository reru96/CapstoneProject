using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomNode 
{
    public Vector3Int gridPos;
    public bool isStart;
    public bool isEnd;
    public bool isBoss;
    public bool isRest;
    public RoomBounds roomBounds;
    public RoomController controller;
    public Dictionary<Direction, RoomNode> neighbors;

    public RoomNode(Vector3Int pos)
    {
        gridPos = pos;
        isBoss = false;
        isRest = false;
        roomBounds = new RoomBounds(pos, pos);
        neighbors = new Dictionary<Direction, RoomNode>();
    }
    public void AddNeighbor(Direction dir, RoomNode neighbor)
    {
        if (!neighbors.ContainsKey(dir))
            neighbors.Add(dir, neighbor);
    }

    public RoomNode GetNeighbor(Direction dir)
    {
        neighbors.TryGetValue(dir, out RoomNode neighbor);
        return neighbor;
    }

}

public enum Direction
{
    North,
    South,
    East,
    West
}
public struct RoomBounds
{
    public Vector3Int min; 
    public Vector3Int max;

    public RoomBounds(Vector3Int min, Vector3Int max)
    {
        this.min = min;
        this.max = max;
    }

    public bool Intersects(RoomBounds other)
    {
        return !(max.x < other.min.x || min.x > other.max.x ||
                 max.z < other.min.z || min.z > other.max.z);
    }
}
