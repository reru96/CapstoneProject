using System.Collections;
using System.Collections.Generic;
using UnityEditor.Graphs;
using UnityEngine;

[CreateAssetMenu(menuName = ("RPG/Inventory/ShopItem"))]
public class SOShopItem : SOItem
{
    public float value;
    public int cost;
    public StatType statToModify;
    public int amountToModify = 1;

    public void Apply(PlayerStats stats)
    {
        if (stats == null) return;

        string statString = StatTypeToString(statToModify);

        for (int i = 0; i < amountToModify; i++)
            stats.AllocateStatPoint(statString);
    }

    private string StatTypeToString(StatType stat)
    {
        switch (stat)
        {
            case StatType.Strength: return "Strength";
            case StatType.Dexterity: return "Dexterity";
            case StatType.Intelligence: return "Intelligence";
            case StatType.Faith: return "Faith";
            case StatType.Arcane: return "Arcane";
            case StatType.Health: return "Health";
            case StatType.Mana: return "Mana";
            case StatType.Stamina: return "Stamina";
            default: return "";
        }
    }
}
public enum StatType
{
    Strength,
    Dexterity,
    Intelligence,
    Faith,
    Arcane,
    Health,
    Mana,
    Stamina
}


