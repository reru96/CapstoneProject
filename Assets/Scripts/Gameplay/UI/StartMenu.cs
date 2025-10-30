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

    private void Start()
    {
        settingsMenu.SetActive(false);

        var inventory = ServiceLocator.Get<InventoryManager>()?.permanentInventory;
        if (inventory != null)
        {
            ServiceLocator.Get<InventoryManager>().LoadPermanentInventory();
        }

        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.LoadCoins();
        }
    }

    public void NewGame()
    {
        var inventoryManager = ServiceLocator.Get<InventoryManager>();
        if (inventoryManager != null)
        {
            inventoryManager.permanentInventory.unlockedUpgrades.Clear();
            inventoryManager.permanentInventory.equippedUpgrades.Clear();
            inventoryManager.SavePermanentInventory();
        }

        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.SetCoins(0);
        }

        SaveData newSave = new SaveData
        {
            coins = 0
        };
        SaveSystem.Save(newSave);

        SceneManager.LoadScene("ClassSelection");
    }

    public void ContinueGame()
    {
        var inventoryManager = ServiceLocator.Get<InventoryManager>();
        if (inventoryManager != null)
        {
            inventoryManager.LoadPermanentInventory();
        }

        if (CoinManager.Instance != null)
        {
            CoinManager.Instance.LoadCoins();
        }

        SaveData data = SaveSystem.Load();
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
