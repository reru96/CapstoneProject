using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager :Singleton<InputManager>
{
    public MyInput config;

    protected override bool ShouldBeDestroyOnLoad() => false;

    protected override void Awake()
    {
        base.Awake();

        LoadConfig();
    }

    public void SetKey(string action, KeyCode newKey)
    {
        switch (action)
        {
            case "Dodge": config.dodge = newKey; break;
            case "Attack": config.attack = newKey; break;
            case "Move": config.move = newKey; break;
            case "Pause": config.pause = newKey; break; 
            case "Ability1": config.ability_1 = newKey; break;
            case "Ability2": config.ability_2 = newKey; break;
            case "Ability3": config.ability_3 = newKey; break;
        }

        SaveConfig();
    }

    private void SaveConfig()
    {
        PlayerPrefs.SetString("MyInput", JsonUtility.ToJson(config));
    }

    private void LoadConfig()
    {
        if (PlayerPrefs.HasKey("MyInput"))
        {
            config = JsonUtility.FromJson<MyInput>(PlayerPrefs.GetString("MyInput"));
        }
    }
}
