using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem 
{
    private static string path = Application.persistentDataPath + "/saveData.json";

    public static void SavePermanentInventory(PermanentInventory inventory)
    {
        SaveData data = new SaveData
        {
            unlockedUpgrades = inventory.unlockedUpgrades.ConvertAll(u => u.name),
            equippedUpgrades = inventory.equippedUpgrades.ConvertAll(e => e.name)
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
    }

    public static void LoadPermanentInventory(PermanentInventory inventory, List<SOShopItem> allItems)
    {
        if (!File.Exists(path))
        {
            Debug.Log("Nessun salvataggio trovato, nuovo inventario creato.");
            return;
        }

        string json = File.ReadAllText(path);
        SaveData data = JsonUtility.FromJson<SaveData>(json);

        inventory.unlockedUpgrades.Clear();
        inventory.equippedUpgrades.Clear();

        foreach (var itemName in data.unlockedUpgrades)
        {
            SOShopItem item = allItems.Find(i => i.name == itemName);
            if (item != null)
                inventory.unlockedUpgrades.Add(item);
        }

        foreach (var itemName in data.equippedUpgrades)
        {
            SOShopItem item = allItems.Find(i => i.name == itemName);
            if (item != null)
                inventory.equippedUpgrades.Add(item);
        }
    }
}
