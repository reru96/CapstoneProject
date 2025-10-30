using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using Gameplay;
using UnityEngine.UIElements;
using UnityEngine.UI;
using TMPro;

public class TreasurePickUp : MonoBehaviour
{
    public List<SOTreasureChestData> classChestData; 
    public UITreasure uiManager;
    public GameObject top;

    private bool playerInRange = false;
    private bool chestOpened = false;

    private void Update()
    {
        if (!playerInRange || chestOpened)
            return;

        var inputManager = ServiceLocator.Get<InputManager>();
        if (Input.GetKeyDown(inputManager.config.action))
        {
            OpenChest();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            var inputManager = ServiceLocator.Get<InputManager>();
            var gameUIManager = ServiceLocator.Get<GameUIManager>();
            gameUIManager?.ShowActionPrompt($"Press {inputManager.config.action} to open");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var gameUIManager = ServiceLocator.Get<GameUIManager>();
            playerInRange = false;
            gameUIManager?.HideActionPrompt();
            uiManager?.Hide();
        }
    }

    public void OpenChest()
    {
        if (chestOpened) return;
        chestOpened = true;

        if (top != null)
            top.transform.Rotate(-90f, 0, 0);

        var gameUIManager = ServiceLocator.Get<GameUIManager>();
        gameUIManager?.HideActionPrompt();

        var player = GameObject.FindGameObjectWithTag("Player");
        var stateMachine = player?.GetComponent<PlayerStateMachine>();
        ClassType playerClassType = stateMachine != null ? stateMachine.p_data.classType : ClassType.Warrior;


        SOTreasureChestData chosenChest = classChestData.Find(chest => chest.classType == playerClassType);

        if (chosenChest == null && classChestData.Count > 0)
            chosenChest = classChestData[0];

        var items = chosenChest != null ? chosenChest.GetRandomItems() : new List<SORunItem>();

        gameUIManager?.ShowTreasure(items);
        uiManager?.ShowChoices(items);
    }
}
