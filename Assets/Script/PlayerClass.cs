using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewClass", menuName = "RPG/PlayerClass")]
public class SOPlayerClass: ScriptableObject
{
    public string nameClass;
    public int maxHP;
    public int maxMana;
    public int strength;
    public int intelligence;
    public GameObject prefab;
}
