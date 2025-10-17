using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class InputRebindUI : MonoBehaviour
{
    [Header("UI Parent Panel")]
    [SerializeField] private Transform buttonParent;
    [SerializeField] private GameObject buttonPrefab;

    private readonly Dictionary<string, Button> buttons = new();
    private InputManager inputManager;
    private Button waitingForButton;

    private void Awake()
    {
        inputManager = ServiceLocator.Get<InputManager>();

        if (buttonParent == null)
        {
            Debug.LogError("[InputRebindUI] Nessun parent assegnato per i pulsanti!");
            return;
        }

        if (buttonPrefab == null)
        {
            Debug.LogError("[InputRebindUI] Nessun prefab assegnato per i pulsanti!");
            return;
        }

        CreateButtons();
        UpdateButtonLabels();
    }

    private void Update()
    {
        if (waitingForButton != null)
        {
            foreach (KeyCode key in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(key))
                {
                    string actionName = waitingForButton.name;
                    inputManager.RebindKey(actionName, key);
                    PlayerPrefs.SetInt(actionName, (int)key);
                    PlayerPrefs.Save();

                    waitingForButton = null;
                    UpdateButtonLabels();
                    break;
                }
            }
        }
    }

    private void CreateButtons()
    {
        string[] actions =
        {
                "Dodge", "Attack", "Move", "Pause",
                "SwitchWeapon", "Ability1", "Ability2", "Ability3"
            };

        foreach (string action in actions)
        {
            var buttonGO = Instantiate(buttonPrefab, buttonParent);
            buttonGO.name = action;

            var text = buttonGO.GetComponentInChildren<Text>();
            if (text != null) text.text = $"{action}: ...";

            var button = buttonGO.GetComponent<Button>();
            buttons[action] = button;

            button.onClick.AddListener(() => StartRebind(action, button));
        }
    }

    private void StartRebind(string actionName, Button button)
    {
        waitingForButton = button;
        button.GetComponentInChildren<Text>().text = $"{actionName}: Press any key...";
    }

    private void UpdateButtonLabels()
    {
        foreach (var kvp in buttons)
        {
            string action = kvp.Key;
            Button button = kvp.Value;
            var text = button.GetComponentInChildren<Text>();

            if (text == null) continue;

            KeyCode key = action switch
            {
                "Dodge" => inputManager.config.dodge,
                "Attack" => inputManager.config.attack,
                "Move" => inputManager.config.move,
                "Pause" => inputManager.config.pause,
                "SwitchWeapon" => inputManager.config.switchWeapon,
                "Ability1" => inputManager.config.ability_1,
                "Ability2" => inputManager.config.ability_2,
                "Ability3" => inputManager.config.ability_3,
                _ => KeyCode.None
            };

            text.text = $"{action}: {key}";
        }
    }
}