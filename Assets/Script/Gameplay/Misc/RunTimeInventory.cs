using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class RunTimeInventory 
{
    public List<SORunItem> items = new List<SORunItem>();
    public SORunItem equippedItem;
    public List<SORunItem> consumables = new List<SORunItem>();
    public int maxSlots = 10;

    public void AddItem(SORunItem item)
    {
        if (items.Count < maxSlots)
            items.Add(item);
    }

    public void EquipItem(SORunItem equipped)
    {
        if (equipped.itemType == ItemType.Equipable) 
         equippedItem = equipped;
    }

    public void UseConsumable(SORunItem consumable)
    {
        if (consumables.Contains(consumable))
        {
            //consumable.Use();
            consumables.Remove(consumable);
        }

    }
    public void ResetInventory()
    {
        items.Clear();
        consumables.Clear();
        equippedItem = null;
    }

}
