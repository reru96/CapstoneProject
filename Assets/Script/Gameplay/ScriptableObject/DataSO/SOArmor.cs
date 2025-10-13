using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("RPG/Armor"))]
public class SOArmor : ScriptableObject
{
    public string armorName;
    public GameObject armorPrefab;
    public Sprite icon;
    public float defenseBonus;
    public float poiseBonus;

    public float strengthBonus;
    public float dexterityBonus;
    public float intelligenceBonus;
    public float faithBonus;
    public float arcaneBonus;
}
