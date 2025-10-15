using System.Collections;
using System.Collections.Generic;
using Codice.Client.BaseCommands.Merge.Xml;
using UnityEngine;
using UnityEngine.Rendering;

[CreateAssetMenu(menuName =("RPG/Inventory/Item"))]
public class SOItem : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public ItemType itemType;
    public Sprite icon;
}
