using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;

public class StatueShop : MonoBehaviour
{
    [Header("Shop Settings")]
    [SerializeField] private string actionMessage;

    private GameUIManager gameUI;
    [SerializeField]private ShopUI shopUI;
    private InputManager inputManager;
    private PlayerStats playerStats;
    private InventoryManager inventoryManager;

    private bool playerInRange = false;
    private bool shopOpen = false;
    private Transform playerTransform;

    private void Start()
    {
        gameUI = ServiceLocator.Get<GameUIManager>();
        inputManager = ServiceLocator.Get<InputManager>();
        inventoryManager = ServiceLocator.Get<InventoryManager>();
        shopUI = gameUI.GetComponent<ShopUI>();

        var spawnManager = ServiceLocator.Get<PlayerSpawnManager>();
        if (spawnManager != null)
            playerTransform = spawnManager.Player?.transform;

        if (playerTransform != null)
            playerStats = playerTransform.GetComponent<PlayerStats>();
    }

    private void Update()
    {
        if (!playerInRange || shopUI == null || inputManager == null)
            return;

        if (Input.GetKeyDown(inputManager.config.action))
        {
            if (!shopOpen)
                OpenShop();
            else
                CloseShop();
        }
    }

    private void OpenShop()
    {
        if (shopUI == null || inventoryManager == null || playerStats == null)
            return;

        shopUI.Show();
        shopUI.Initialize(inventoryManager.permanentInventory, playerStats);

        gameUI.HideActionPrompt();
        shopOpen = true;
    }

    private void CloseShop()
    {
        if (shopUI == null) return;

        shopUI.Hide();
        shopOpen = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            actionMessage = $"Press {inputManager.config.action}";
            playerInRange = true;
            gameUI?.ShowActionPrompt(actionMessage);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            gameUI?.HideActionPrompt();

            if (shopOpen)
                CloseShop();
        }
    }
}
