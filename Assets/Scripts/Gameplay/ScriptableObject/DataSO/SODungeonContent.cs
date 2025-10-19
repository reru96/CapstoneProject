using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonRoomContent", menuName = "Dungeon/Room Content")]
public class SODungeonContent : ScriptableObject
{
    public List<ContentEntry> contents = new List<ContentEntry>();
}

[System.Serializable]
public class ContentEntry
{
    public GameObject prefab;
    public Vector3 localPosition;
    public Vector3 localRotation;
}