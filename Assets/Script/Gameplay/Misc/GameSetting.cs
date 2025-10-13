using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameSettings
{
    private const string SELECTED_CLASS_KEY = "SelectedClass";
    private const string PLAYER_PROGRESS_KEY = "PlayerProgress";

    public static SOPlayerClass SelectedClass { get; set; }

    public static int PlayerLevel { get; private set; } = 1;
    public static int PlayerExperience { get; private set; }
    public static int UnlockedClasses { get; private set; } = 1; 
    
    public static void SaveSettings()
    {
        if (SelectedClass != null)
        {
            PlayerPrefs.SetString(SELECTED_CLASS_KEY, SelectedClass.name);
        }

        PlayerPrefs.SetInt("PlayerLevel", PlayerLevel);
        PlayerPrefs.SetInt("PlayerExperience", PlayerExperience);
        PlayerPrefs.SetInt("UnlockedClasses", UnlockedClasses);
        PlayerPrefs.Save();

        Debug.Log("[GameSettings] Impostazioni salvate.");
    }

   
    public static void LoadSettings()
    {
       
        if (PlayerPrefs.HasKey(SELECTED_CLASS_KEY))
        {
            string className = PlayerPrefs.GetString(SELECTED_CLASS_KEY);
            
        }

        PlayerLevel = PlayerPrefs.GetInt("PlayerLevel", 1);
        PlayerExperience = PlayerPrefs.GetInt("PlayerExperience", 0);
        UnlockedClasses = PlayerPrefs.GetInt("UnlockedClasses", 1);

        Debug.Log("[GameSettings] Impostazioni caricate.");
    }

    
    public static bool IsClassUnlocked(int classIndex)
    {
        return classIndex < UnlockedClasses;
    }

    public static void UnlockClass()
    {
        UnlockedClasses++;
        SaveSettings();
        Debug.Log($"[GameSettings] Nuova classe sbloccata! Totale: {UnlockedClasses}");
    }

    
    public static void ResetSettings()
    {
        SelectedClass = null;
        PlayerLevel = 1;
        PlayerExperience = 0;
        UnlockedClasses = 1;

        PlayerPrefs.DeleteAll();
        Debug.Log("[GameSettings] Impostazioni resettate.");
    }
}
