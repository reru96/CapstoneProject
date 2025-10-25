using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InventoryPanel
{
    public string panelName;
    public UIInventorySlot[] slots;
    public PanelType type;
}

public enum PanelType
{
    Weapons,
    BaseWeapons,
    Consumables,
    Upgrades
}
