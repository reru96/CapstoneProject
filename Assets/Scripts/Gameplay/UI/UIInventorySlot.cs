using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIInventorySlot : MonoBehaviour
{
    public Image icon;
    public TextMeshProUGUI label;

    private SOShopItem currentItem;

    public void SetItem(SOShopItem item)
    {
        currentItem = item;
        if (icon != null) icon.sprite = item.icon;
        if (label != null) label.text = item.itemName;
        gameObject.SetActive(true);
    }

    public void Clear()
    {
        currentItem = null;
        if (icon != null) icon.sprite = null;
        if (label != null) label.text = "";
        gameObject.SetActive(false);
    }
}
