using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Dungeon/DungeonConfig")]
public class DungeonConfig : ScriptableObject
{
    public List<SORoom> rooms;
    public int roomCount = 10;
}
