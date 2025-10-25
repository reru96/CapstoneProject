using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Room
{
    public RoomType type;
    public Vector2Int bottomLeft;
    public Vector2Int topRight;

    public Room(RoomType type, Vector2Int bottomLeft, Vector2Int topRight)
    {
        this.type = type;
        this.bottomLeft = bottomLeft;
        this.topRight = topRight;
    }

    public Vector3 GetCenter()
    {
        return new Vector3(
            (bottomLeft.x + topRight.x) / 2f,
            0,
            (bottomLeft.y + topRight.y) / 2f
        );
    }

    public Vector2Int GetCenter2D()
    {
        return new Vector2Int((bottomLeft.x + topRight.x) / 2, (bottomLeft.y + topRight.y) / 2);
    }
}