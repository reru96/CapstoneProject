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

    public List<SOWeapon> GetWeapons()
    {
        List<SOWeapon> weapons = new List<SOWeapon>();
        foreach (var item in items)
        {
            if (item is SOWeapon weapon)
                weapons.Add(weapon);
        }
        return weapons;
    }

    public int GetCurrentWeaponIndex()
    {
        var weapons = GetWeapons();
        if (equippedItem == null || !(equippedItem is SOWeapon eq))
            return -1;
        return weapons.IndexOf(eq);
    }

    public SOWeapon CycleWeapon(int direction)
    {
        var weapons = GetWeapons();
        if (weapons.Count == 0) return null;

        int currentIndex = GetCurrentWeaponIndex();
        if (currentIndex < 0) currentIndex = 0;

        int newIndex = (currentIndex + direction + weapons.Count) % weapons.Count;
        var newWeapon = weapons[newIndex];
        EquipItem(newWeapon);
        return newWeapon;
    }
}
