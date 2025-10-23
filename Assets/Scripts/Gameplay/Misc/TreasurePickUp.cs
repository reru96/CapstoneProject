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
    public SOTreasureChestData chestData;
    public UITreasure uiManager;
    public GameObject top;
    public GameObject actionButton;

    private bool playerInRange = false;
    private InputManager inputManager;
    private bool chestOpened = false;

    private void Start()
    {
        inputManager = ServiceLocator.Get<InputManager>();
        if (actionButton != null)
            actionButton.SetActive(false);
    }

    private void Update()
    {
        if (!playerInRange || chestOpened)
            return;

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

            if (actionButton != null)
            {
                actionButton.SetActive(true);
                TextMeshProUGUI text = actionButton.GetComponentInChildren<TextMeshProUGUI>();
                if (text != null)
                    text.text = $"Press {inputManager.config.action}";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            if (actionButton != null)
                actionButton.SetActive(false);

            if (uiManager != null)
                uiManager.Hide(); 
        }
    }

    public void OpenChest()
    {
        if (chestOpened)
            return;

        chestOpened = true;

        if (top != null)
            top.transform.Rotate(-90f, 0, 0);

        var items = chestData.GetRandomItems();
        uiManager?.ShowChoices(items);
    }
}
