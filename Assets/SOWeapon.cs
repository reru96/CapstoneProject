using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName =("RPG/Weapon"))]
public class SOWeapon : ScriptableObject
{
    public string nameWeapon;
    public Sprite icon;
    public GameObject prefab;

    [Header("Stats")]
   
    public float strengthBonus;
    public float intelligenceBonus;
    public float dexterityBonus;
    public float defenseBonus;
    public float speedBonus;
}
