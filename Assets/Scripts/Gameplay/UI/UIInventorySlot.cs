using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIInventorySlot : MonoBehaviour
{
    public Image icon;

    private SORunItem runItem;
    private SOShopItem shopItem;

    public void SetItem(SORunItem item)
    {
        runItem = item;
        shopItem = null;

        if (item != null)
        {
            icon.sprite = item.icon;
            icon.enabled = true;
        }
        else
        {
            icon.sprite = null;
            icon.enabled = false;
        }
    }

    public void SetItem(SOShopItem passive)
    {
        shopItem = passive;
        runItem = null;

        if (shopItem != null)
        {
            icon.sprite = shopItem.icon;
            icon.enabled = true;
        }
        else
        {
            icon.sprite = null;
            icon.enabled = false;
        }
    }

    public void Clear()
    {
        runItem = null;
        shopItem = null;
        icon.sprite = null;
        icon.enabled = false;
    }

    public SORunItem GetRunItem() => runItem;
    public SOShopItem GetShopItem() => shopItem;
}
