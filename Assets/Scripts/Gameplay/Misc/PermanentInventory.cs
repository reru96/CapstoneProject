using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PermanentInventory 
{
    public List<SOShopItem> unlockedUpgrades = new List<SOShopItem>();
    public List<SOShopItem> equippedUpgrades = new List<SOShopItem>();
    public int maxEquipped = 3;

    public void UnlockUpgrade(SOShopItem upgrade)
    {
        if (!unlockedUpgrades.Contains(upgrade))
            unlockedUpgrades.Add(upgrade);
    }

    public void EquipUpgrade(SOShopItem upgrade)
    {
        if (unlockedUpgrades.Contains(upgrade) && equippedUpgrades.Count < maxEquipped)
            equippedUpgrades.Add(upgrade);
    }

    public void UnequipUpgrade(SOShopItem upgrade)
    {
        equippedUpgrades.Remove(upgrade);
    }
}
