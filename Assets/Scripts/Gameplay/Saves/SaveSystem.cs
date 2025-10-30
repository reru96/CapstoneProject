using System.Collections.Generic;
using System.IO;
using Core;
using Gameplay;
using UnityEngine;

public static class SaveSystem
{
    private static string savePath = Application.persistentDataPath + "/save.json";

    public static void Save(SaveData data)
    {
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(savePath, json);
        Debug.Log("[SaveSystem] Salvataggio completato!");
    }

    public static SaveData Load()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            SaveData data = JsonUtility.FromJson<SaveData>(json);
            Debug.Log("[SaveSystem] Salvataggio caricato!");
            return data;
        }
        else
        {
            Debug.Log("[SaveSystem] Nessun salvataggio trovato, creando nuovo SaveData.");
            return new SaveData(); 
        }
    }

    public static void DeleteSave()
    {
        if (File.Exists(savePath))
        {
            File.Delete(savePath);
            Debug.Log("[SaveSystem] Salvataggio eliminato.");
        }
    }
}