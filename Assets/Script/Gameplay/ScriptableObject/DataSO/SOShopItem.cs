using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("RPG/Inventory/ShopItem"))]
public class SOShopItem : SOItem
{
    public string bonusType;
    public float value;
    public int cost;
}
