using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName =("Room"), menuName =("RPG/Rooms"))]
public class SORoom : ScriptableObject
{
    public string nameRoom;
    public int id;

    public List<GameObject> floorPrefabs = new List<GameObject>();
    public GameObject wallPrefab;
    public GameObject doorPrefab;

    public int roomWidth = 3;
    public int roomLength = 3;
    public float tileSize = 5f;
    public float offset = 0.2f;
    public float floorHeight = 0f;
    public float floorSpacing = 0f;
    public float wallOffsetX = 0f; 
    public float wallOffsetZ = 0f;
    public float wallSpacing = 0f;
    public float wallOffset = 0f;

    public bool hasCorners = true;
}
