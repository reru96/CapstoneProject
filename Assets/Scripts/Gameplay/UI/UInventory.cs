using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;

public class UInventory : MonoBehaviour
{
    public InventoryPanel[] panels;
    public CanvasGroup inventoryCanvasGroup;
    private InventoryManager _inventoryManager;

    private void Awake()
    {
        _inventoryManager = ServiceLocator.Get<InventoryManager>();
        RefreshAllPanels();
        SetInventoryVisibility(false);
    }

    public void SetInventoryVisibility(bool visible)
    {
        if (inventoryCanvasGroup == null) return;
        inventoryCanvasGroup.alpha = visible ? 1f : 0f;
        inventoryCanvasGroup.interactable = visible;
        inventoryCanvasGroup.blocksRaycasts = visible;
    }

    public void RefreshAllPanels()
    {
        foreach (var panel in panels)
        {
            switch (panel.type)
            {
                case PanelType.Weapons:
                    RefreshRunItemsPanel(panel, _inventoryManager.runInventory.items);
                    break;
                case PanelType.Consumables:
                    RefreshRunItemsPanel(panel, _inventoryManager.runInventory.consumables);
                    break;
                case PanelType.Upgrades:
                    RefreshPassivePanel(panel, _inventoryManager.permanentInventory.equippedUpgrades);
                    break;
            }
        }
    }

    private void RefreshRunItemsPanel(InventoryPanel panel, List<SORunItem> items)
    {
        for (int i = 0; i < panel.slots.Length; i++)
        {
            if (i < items.Count)
                panel.slots[i].SetItem(items[i]);
            else
                panel.slots[i].Clear();
        }
    }

    private void RefreshPassivePanel(InventoryPanel panel, List<SOShopItem> passives)
    {
        for (int i = 0; i < panel.slots.Length; i++)
        {
            if (i < passives.Count)
                panel.slots[i].SetItem(passives[i]);
            else
                panel.slots[i].Clear();
        }
    }
}
