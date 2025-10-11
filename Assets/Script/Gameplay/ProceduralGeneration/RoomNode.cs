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
}
