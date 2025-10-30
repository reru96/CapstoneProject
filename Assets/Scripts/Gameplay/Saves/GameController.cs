using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;


public class GameController : Singleton<GameController>
{
    [Header("Inventory")]
    public PermanentInventory permanentInventory;
    public List<SOShopItem> allShopItems;

    protected override bool ShouldBeDestroyOnLoad() => false;

    protected override void Awake()
    {
        base.Awake();

        if (permanentInventory == null)
            permanentInventory = new PermanentInventory();

        LoadGame();
    }

    public void SaveGame()
    {
        var coinManager = CoinManager.Instance;
        int coins = coinManager != null ? coinManager.GetCoins() : 0;

        SaveData data = new SaveData
        {
            coins = coins,
            unlockedUpgrades = permanentInventory.unlockedUpgrades.ConvertAll(u => u.name),
            equippedUpgrades = permanentInventory.equippedUpgrades.ConvertAll(e => e.name)
        };

        SaveSystem.Save(data);
        Debug.Log("[GameController] Gioco salvato!");
    }

    public void LoadGame()
    {
        SaveData data = SaveSystem.Load();

        permanentInventory.unlockedUpgrades.Clear();
        permanentInventory.equippedUpgrades.Clear();

        foreach (var name in data.unlockedUpgrades)
        {
            SOShopItem item = allShopItems.Find(i => i.name == name);
            if (item != null)
                permanentInventory.unlockedUpgrades.Add(item);
        }

        foreach (var name in data.equippedUpgrades)
        {
            SOShopItem item = allShopItems.Find(i => i.name == name);
            if (item != null)
                permanentInventory.equippedUpgrades.Add(item);
        }

        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.SetCoins(data.coins);
        }

        Debug.Log($"[GameController] Gioco caricato! Coins: {data.coins}");
    }

    protected override void OnApplicationQuit()
    {
        SaveGame();
        base.OnApplicationQuit();
        
    }
}
