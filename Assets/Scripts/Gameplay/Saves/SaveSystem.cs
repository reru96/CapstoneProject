using System.Collections.Generic;
using System.IO;
using Core;
using Gameplay;
using UnityEngine;

public static class SaveSystem
{

    private static string path = Application.persistentDataPath + "/saveData.json";

    public static void SavePermanentInventory(PermanentInventory inventory, int coins)
    {
        SaveData data = new SaveData
        {
            unlockedUpgrades = inventory.unlockedUpgrades.ConvertAll(u => u.name),
            equippedUpgrades = inventory.equippedUpgrades.ConvertAll(e => e.name),
            coin = coins
        };

        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log($"[SaveSystem] Salvataggio completato. Coins: {coins}");
    }

    public static int LoadPermanentInventory(PermanentInventory inventory, List<SOShopItem> allItems)
    {
        if (!File.Exists(path))
        {
            Debug.Log("Nessun salvataggio trovato, nuovo inventario creato.");
            return 0;
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

        Debug.Log($"[SaveSystem] Caricamento completato. Coins: {data.coin}");
        return data.coin;
    }
}