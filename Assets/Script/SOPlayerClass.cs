using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewClass", menuName = "RPG/PlayerClass")]
public class SOPlayerClass: ScriptableObject
{

    public string nameClass;
    [Header("Stats")]
    public int maxHP;
    public int maxMana;
    public int maxStamina;
    public int strength;
    public int intelligence;
    public int dexterity;
    public int defense;
    public int speed;
    public GameObject prefab;
}
