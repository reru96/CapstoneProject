using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIInventorySlot : MonoBehaviour
{
    public Image icon;
    private SORunItem runItem;
    private SOShopItem shopItem;
    private InventoryManager _inventoryManager;
    [Range(0f, 1f)] public float unequippedDarkness = 0.4f;
    private void Start()
    {
        _inventoryManager = ServiceLocator.Get<InventoryManager>();
    }

    public void SetItem(SORunItem item)
    {
        runItem = item;
        shopItem = null;
        UpdateIcon(item?.icon);
    }

    public void SetItem(SOShopItem passive)
    {
        shopItem = passive;
        runItem = null;
        UpdateIcon(passive?.icon);
    }

    private void UpdateIcon(Sprite sprite)
    {
        if (icon == null) return;

        if (sprite != null)
        {
            icon.sprite = sprite;
            icon.enabled = true;
        }
        else
        {
            icon.sprite = null;
            icon.enabled = false;
        }
    }

    public void OnClick()
    {
        if (shopItem == null)
            return; 

        var inv = _inventoryManager.permanentInventory;

        if (inv.equippedUpgrades.Contains(shopItem))
        {
            _inventoryManager.UnequipUpgrade(shopItem);
            Debug.Log($"Unequipped: {shopItem.name}");
        }
        else
        {
            if (inv.equippedUpgrades.Count < inv.maxEquipped)
            {
                _inventoryManager.EquipUpgrade(shopItem);
                Debug.Log($"Equipped: {shopItem.name}");
            }
            else
            {
                Debug.Log("Nessuno slot disponibile per altri upgrade!");
            }
        }

        UpdateVisualState();
    }

    private void UpdateVisualState()
    {
        if (shopItem == null || icon == null)
            return;

        bool equipped = _inventoryManager.permanentInventory.equippedUpgrades.Contains(shopItem);
        icon.color = equipped
            ? new Color(1f, 1f, 1f, 1f)                 
            : new Color(1f - unequippedDarkness, 1f - unequippedDarkness, 1f - unequippedDarkness, 1f);
    }

    public void Clear()
    {
        runItem = null;
        shopItem = null;
        UpdateIcon(null);
    }

    public SORunItem GetRunItem() => runItem;
    public SOShopItem GetShopItem() => shopItem;
}
