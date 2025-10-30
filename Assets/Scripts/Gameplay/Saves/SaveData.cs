using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]   
public class SaveData 
{
    public List<string> unlockedUpgrades = new();
    public List<string> equippedUpgrades = new();
    public int coins = 0;
}
