using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public GameObject settingsMenu;
    public GameObject VolumePanel;
    public GameObject InputPanel; 
    private SaveData lastSave;

    private void Start()
    {
        settingsMenu.SetActive(false); 
        var inventory = ServiceLocator.Get<InventoryManager>()?.permanentInventory;
        if (inventory != null)
        {
            SaveSystem.LoadPermanentInventory(inventory, ServiceLocator.Get<InventoryManager>().allShopItems);
        }

 
        lastSave = new SaveData();
        var gameManager = ServiceLocator.Get<GameManager>();
        if (gameManager != null)
        {
            lastSave.coin = gameManager.Coins;
        }
    }

    public void NewGame()
    {
        var inventory = ServiceLocator.Get<InventoryManager>()?.permanentInventory;
        if (inventory != null)
        {
            inventory.unlockedUpgrades.Clear();
            inventory.equippedUpgrades.Clear();
        }

        var gameManager = ServiceLocator.Get<GameManager>();
        if (gameManager != null)
        {
            gameManager.SetCoins(0);
        }

        SaveData newSave = new SaveData
        {
            coin = 0
        };

        SaveSystem.SavePermanentInventory(inventory, 0);

        SceneManager.LoadScene(1);
    }

    public void ContinueGame()
    {
        var inventory = ServiceLocator.Get<InventoryManager>()?.permanentInventory;
        if (inventory != null)
        {
            SaveSystem.LoadPermanentInventory(inventory, ServiceLocator.Get<InventoryManager>().allShopItems);
        }

        SceneManager.LoadScene("ClassSelection");
    }

    public void ShowOptions()
    {
        settingsMenu.SetActive(true);
    }

    public void HideOptions()
    {
        settingsMenu.SetActive(false);
    }

    public void ShowInputPanel()
    { 
        InputPanel.SetActive(true);
        VolumePanel.SetActive(false);
    }
    public void ShowVolumePanel()
    {  
        VolumePanel.SetActive(true);
        InputPanel.SetActive(false);
    }

    public void OnExitGame()
    {
        Application.Quit();
    }
}
