using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DungeonRoomContent", menuName = "Dungeon/Room Content")]
public class SODungeonContent : ScriptableObject
{
    public RoomType roomType; 
    public List<GameObject> possibleContents;
}

