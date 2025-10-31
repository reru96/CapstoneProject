using UnityEngine;

[CreateAssetMenu(menuName =("RPG/Inventory/Item"))]
public class SOItem : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public ItemType itemType;
    public Sprite icon;
}
