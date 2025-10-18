using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomNode : Node
{
    public Vector3Int gridPos;
    public bool isStart;
    public bool isBoss;
    public bool isRest;

    public RoomNode(Vector2Int bottomLeftAreaCorner, Vector2Int topRightAreaCorner, Node parentNode, int index) : base(parentNode)
    {
        this.BottomLeftAreaCorner = bottomLeftAreaCorner;
        this.TopRightAreaCorner = topRightAreaCorner;
        this.BottomRightAreaCorner = new Vector2Int(topRightAreaCorner.x, bottomLeftAreaCorner.y);
        this.TopLeftAreaCorner = new Vector2Int(bottomLeftAreaCorner.x, topRightAreaCorner.y);
        this.TreeLayerIndex = index;
    }

    public Vector3 CenterPosition()
    {
        float centerX = BottomLeftAreaCorner.x + Width / 2f;
        float centerZ = BottomLeftAreaCorner.y + Length / 2f;
        return new Vector3(centerX, 0, centerZ);
    }

    public int Width { get => (int)(TopRightAreaCorner.x - BottomLeftAreaCorner.x); }
    public int Length { get => (int)(TopRightAreaCorner.y - BottomLeftAreaCorner.y); }

}

