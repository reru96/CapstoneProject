using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using System;

namespace Gameplay
{
    public class InventoryManager : Injectable<InventoryManager>
    {
        public RunTimeInventory runInventory { get; private set; }
        public PermanentInventory permanentInventory { get; private set; }

        public List<SOShopItem> allShopItems;

        public event Action OnInventoryChanged;

        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);

            if (runInventory == null)
                runInventory = new RunTimeInventory();

            if (permanentInventory == null)
                permanentInventory = new PermanentInventory();

            LoadPermanentInventory();
        }

        public void ResetRunInventory()
        {
            runInventory.ResetInventory();
            OnInventoryChanged?.Invoke();
        }

        public void EquipUpgrade(SOShopItem upgrade)
        {
            permanentInventory.EquipUpgrade(upgrade);
            OnInventoryChanged?.Invoke();
        }

        public void UnequipUpgrade(SOShopItem upgrade)
        {
            permanentInventory.UnequipUpgrade(upgrade);
            OnInventoryChanged?.Invoke();
        }

        public void SavePermanentInventory()
        {
            SaveData data = SaveSystem.Load(); 
            data.unlockedUpgrades = permanentInventory.unlockedUpgrades.ConvertAll(u => u.name);
            data.equippedUpgrades = permanentInventory.equippedUpgrades.ConvertAll(e => e.name);

            SaveSystem.Save(data);
            Debug.Log("[InventoryManager] PermanentInventory salvato!");
        }

        public void LoadPermanentInventory()
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

            OnInventoryChanged?.Invoke();
            Debug.Log("[InventoryManager] PermanentInventory caricato!");
        }

        public void ApplyEquippedUpgrades(PlayerStats stats)
        {
            foreach (var item in permanentInventory.equippedUpgrades)
            {
                item.Apply(stats);
            }
        }

        private void OnApplicationQuit()
        {
            SavePermanentInventory();
        }
    }
}