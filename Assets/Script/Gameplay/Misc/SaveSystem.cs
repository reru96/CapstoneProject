using System.IO;
using UnityEngine;

public static class SaveSystem 
{
    private static string path = Application.persistentDataPath + "/passiveInventory.json";

    public static void SavePermanentInventory(PermanentInventory inventory)
    {
        string json = JsonUtility.ToJson(inventory, true);
        File.WriteAllText(path, json);
    }

    public static void LoadPermanentInventory(PermanentInventory inventory)
    {
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            JsonUtility.FromJsonOverwrite(json, inventory);
        }
        else
        {
            Debug.Log("Nessun salvataggio trovato, nuovo inventario creato.");
        }
    }
}
