using System;
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

    private void Start()
    {
        _inventoryManager = ServiceLocator.Get<InventoryManager>();
        _inventoryManager.OnInventoryChanged += RefreshAllPanels;
        SetInventoryVisibility(false);
    }

    private void OnDestroy()
    {
        if (_inventoryManager != null)
            _inventoryManager.OnInventoryChanged -= RefreshAllPanels;
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
            RefreshUpgradePanel(panel, _inventoryManager.permanentInventory.unlockedUpgrades);
        }
    }

    private void RefreshUpgradePanel(InventoryPanel panel, System.Collections.Generic.List<SOShopItem> upgrades)
    {
        for (int i = 0; i < panel.slots.Length; i++)
        {
            if (i < upgrades.Count)
                panel.slots[i].SetItem(upgrades[i]);
            else
                panel.slots[i].Clear();
        }
    }
}
