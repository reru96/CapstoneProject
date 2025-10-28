using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;

namespace Gameplay
{
    public class InventoryManager : Injectable<InventoryManager>
    {
        public RunTimeInventory runInventory { get; private set; }
        public PermanentInventory permanentInventory { get; private set; }

        public List<SOShopItem> allShopItems;

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

        public void ResetRunInventory() => runInventory.ResetInventory();

        public void SavePermanentInventory() =>
            SaveSystem.SavePermanentInventory(permanentInventory);

        public void LoadPermanentInventory()
        {
            SaveSystem.LoadPermanentInventory(permanentInventory, allShopItems);
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