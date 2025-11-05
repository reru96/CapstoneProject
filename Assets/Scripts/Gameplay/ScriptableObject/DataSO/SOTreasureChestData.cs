using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(fileName = "TreasureChest", menuName = "Treasure/TreasureChestData")]
public class SOTreasureChestData : ScriptableObject
{
    public List<SORunItem> allItems;
    public int MaxLevel = 5;
    public int NumberOfChoices = 3;
    public ClassType classType;

    public List<SORunItem> GetRandomItems()
    {
        var validItems = allItems.Where(i => i.level <= MaxLevel).ToList();
        List<SORunItem> chosenItems = new List<SORunItem>();

        for (int i = 0; i < NumberOfChoices && validItems.Count > 0; i++)
        {
            int index = Random.Range(0, validItems.Count);
            chosenItems.Add(validItems[index]);
            validItems.RemoveAt(index);
        }

        return chosenItems;
    }
}