using System.ComponentModel;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [Header("Configurazione input")]
    [SerializeField] private MyInput config;

    public MyInput Config => config;

    private void Awake()
    {
    
        if (Container.Resolver == null)
        {
            Debug.LogWarning("[InputManager] Container.Resolver non trovato");
            return;
        }

        Container.Resolver.RegisterInstance(this);
        LoadConfig();
    }

    private void OnDestroy()
    {
        if (Container.Resolver != null)
            Container.Resolver.UnregisterInstance<InputManager>();
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
            default:
                Debug.LogWarning($"[InputManager] Azione '{action}' non riconosciuta.");
                break;
        }

        SaveConfig();
    }

    private void SaveConfig()
    {
        PlayerPrefs.SetString("MyInput", JsonUtility.ToJson(config));
        PlayerPrefs.Save(); 
    }

    private void LoadConfig()
    {
        if (PlayerPrefs.HasKey("MyInput"))
        {
            config = JsonUtility.FromJson<MyInput>(PlayerPrefs.GetString("MyInput"));
        }
        else
        {
            Debug.Log("[InputManager] Nessuna configurazione input salvata, uso valori di default.");
        }
    }
}
