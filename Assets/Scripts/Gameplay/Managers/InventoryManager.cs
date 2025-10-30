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
            var gameManager = ServiceLocator.Get<GameManager>();
            int coins = gameManager != null ? gameManager.Coins : 0;

            SaveSystem.SavePermanentInventory(permanentInventory, coins);
        }

        public void LoadPermanentInventory()
        {
            int loadedCoins = SaveSystem.LoadPermanentInventory(permanentInventory, allShopItems);

            var gameManager = ServiceLocator.Get<GameManager>();
            if (gameManager != null)
            {
                gameManager.SetCoins(loadedCoins);
            }

            OnInventoryChanged?.Invoke();
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