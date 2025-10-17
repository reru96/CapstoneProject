using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;

public class UInventory : MonoBehaviour
{
    public InventoryPanel[] panels;
    public CanvasGroup inventoryCanvasGroup; 
    private bool isActive;

    private InventoryManager _inventoryManager;
    private InputManager _inputManager;

    private void Awake()
    {
        _inventoryManager = ServiceLocator.Get<InventoryManager>();
        _inputManager = ServiceLocator.Get<InputManager>();

        RefreshAllPanels();
    }

    private void Start()
    {
        SetInventoryVisibility(false);
    }
    private void Update()
    {
        if (Input.GetKeyDown(_inputManager.config.pause))
        {
            isActive = !isActive;
            SetInventoryVisibility(isActive);
            Debug.Log("Inventory toggled: " + isActive);
        }
    }

    private void SetInventoryVisibility(bool visible)
    {
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
