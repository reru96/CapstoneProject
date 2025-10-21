using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "RPG/Inventory/Run Item")]
public class SORunItem : SOItem
{
    public int level;

    public virtual void Use(GameObject player)
    {

    }
}
