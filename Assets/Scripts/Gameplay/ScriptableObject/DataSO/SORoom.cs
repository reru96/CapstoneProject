using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName =("Room"), menuName =("RPG/Rooms"))]
public class SORoom : ScriptableObject
{
    public string roomName;
    public GameObject roomPrefab;
    public Vector2Int roomGridSize = new Vector2Int(1, 1);
    public Vector3 roomWorldSize = new Vector3(10, 3, 10);
    public RoomType roomType;
}
