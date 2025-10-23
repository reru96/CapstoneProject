using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;

public class GameUIManager : Injectable<GameManager>
{
   
    public RestPointUI restPointUI;
    public UITreasure treasureUI;
    public UInventory inventoryUI;
    public UIStatic staticUI;

    private bool isInventoryOpen = false;

    private InventoryManager inventoryManager;

    private void Start()
    {
        HideAll();

        inventoryManager = ServiceLocator.Get<InventoryManager>();
        var player = ServiceLocator.Get<PlayerSpawnManager>().Player;
        if (player != null)
        {
            var life = player.GetComponent<LifeController>();
            var mana = player.GetComponent<ManaController>();
            var stamina = player.GetComponent<StaminaController>();
            var exp = player.GetComponent<PlayerStats>();

            staticUI?.Initialize(life, mana, stamina);
            staticUI?.UpdateExp(exp.exp, exp.expToNextLevel);
        }
    }

    private void Update()
    {
        HandleInventoryToggle();
       
    }

    public void UpdateWeaponUI()
    {
        var currentWeapon = inventoryManager.runInventory.CycleWeapon(0); 
        staticUI.SetWeapon(currentWeapon);
    }

    private void HandleInventoryToggle()
    {
        var inputManager = ServiceLocator.Get<InputManager>();
        if (Input.GetKeyDown(inputManager.config.pause))
        {
            isInventoryOpen = !isInventoryOpen;

            if (isInventoryOpen)
            {
                ShowInventory();
            }
            else
            {
                HideInventory();
            }
        }
    }

    public void ShowInventory()
    {
        
        HideAllMenusExcept(inventoryUI);
        inventoryUI?.SetInventoryVisibility(true);
    }

    public void HideInventory()
    {
        inventoryUI?.SetInventoryVisibility(false);
        isInventoryOpen = false;
    }

    public void ShowLevelUp()
    {
        HideAllMenusExcept(restPointUI);
        restPointUI?.ShowLevelUpPanel();
    }

    public void HideLevelUp()
    {
        restPointUI?.HideLevelUpPanel();
    }

    public void ShowTreasure(List<SORunItem> items)
    {
        HideAllMenusExcept(treasureUI);
        treasureUI?.ShowChoices(items);
    }

    public void HideTreasure()
    {
        treasureUI?.Hide();
    }

    public void ShowRestPrompt(string message)
    {
        restPointUI?.ShowPrompt(message);
    }

    public void HideRestPrompt()
    {
        restPointUI?.HidePrompt();
    }

    public void ShowStaticUI()
    {
        staticUI?.Show();
    }

    public void HideStaticUI()
    {
        staticUI?.Hide();
    }

    private void HideAllMenusExcept(MonoBehaviour exception)
    {
        if (restPointUI != exception)
            restPointUI?.HideLevelUpPanel();

        if (treasureUI != exception)
            treasureUI?.Hide();

        if (inventoryUI != exception)
            inventoryUI?.SetInventoryVisibility(false);
    }

    public void HideAll()
    {
        restPointUI?.HideLevelUpPanel();
        treasureUI?.Hide();
        inventoryUI?.SetInventoryVisibility(false);
        restPointUI?.HidePrompt();
    }

}


