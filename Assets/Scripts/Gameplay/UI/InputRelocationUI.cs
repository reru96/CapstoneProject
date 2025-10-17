using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class InputRebindUI : MonoBehaviour
{
    [Header("UI References")]
    public Button dodgeButton;
    public Button attackButton;
    public Button moveButton;
    public Button pauseButton;
    public Button switchWeaponButton;
    public Button ability1Button;
    public Button ability2Button;
    public Button ability3Button;

    private InputManager inputManager;
    private Button waitingForButton = null;

    private void Awake()
    {
        inputManager = ServiceLocator.Get<InputManager>();

        dodgeButton.onClick.AddListener(() => StartRebind("Dodge", dodgeButton));
        attackButton.onClick.AddListener(() => StartRebind("Attack", attackButton));
        moveButton.onClick.AddListener(() => StartRebind("Move", moveButton));
        pauseButton.onClick.AddListener(() => StartRebind("Pause", pauseButton));
        switchWeaponButton.onClick.AddListener(() => StartRebind("SwitchWeapon", switchWeaponButton));
        ability1Button.onClick.AddListener(() => StartRebind("Ability1", ability1Button));
        ability2Button.onClick.AddListener(() => StartRebind("Ability2", ability2Button));
        ability3Button.onClick.AddListener(() => StartRebind("Ability3", ability3Button));

        LoadKeybinds();
        UpdateButtonLabels();
    }

    private void Update()
    {
        if (waitingForButton != null)
        {

            foreach (KeyCode key in System.Enum.GetValues(typeof(KeyCode)))
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

    private void StartRebind(string actionName, Button button)
    {
        waitingForButton = button;
        button.GetComponentInChildren<Text>().text = "Press any key...";
        button.name = actionName; 
    }

    private void UpdateButtonLabels()
    {
        dodgeButton.GetComponentInChildren<Text>().text = inputManager.config.dodge.ToString();
        attackButton.GetComponentInChildren<Text>().text = inputManager.config.attack.ToString();
        moveButton.GetComponentInChildren<Text>().text = inputManager.config.move.ToString();
        pauseButton.GetComponentInChildren<Text>().text = inputManager.config.pause.ToString();
        switchWeaponButton.GetComponentInChildren<Text>().text = inputManager.config.switchWeapon.ToString();
        ability1Button.GetComponentInChildren<Text>().text = inputManager.config.ability_1.ToString();
        ability2Button.GetComponentInChildren<Text>().text = inputManager.config.ability_2.ToString();
        ability3Button.GetComponentInChildren<Text>().text = inputManager.config.ability_3.ToString();
    }

    private void LoadKeybinds()
    {
        RebindFromPrefs("Dodge", ref inputManager.config.dodge);
        RebindFromPrefs("Attack", ref inputManager.config.attack);
        RebindFromPrefs("Move", ref inputManager.config.move);
        RebindFromPrefs("Pause", ref inputManager.config.pause);
        RebindFromPrefs("SwitchWeapon", ref inputManager.config.switchWeapon);
        RebindFromPrefs("Ability1", ref inputManager.config.ability_1);
        RebindFromPrefs("Ability2", ref inputManager.config.ability_2);
        RebindFromPrefs("Ability3", ref inputManager.config.ability_3);
    }

    private void RebindFromPrefs(string actionName, ref KeyCode key)
    {
        if (PlayerPrefs.HasKey(actionName))
        {
            key = (KeyCode)PlayerPrefs.GetInt(actionName);
        }
    }
}