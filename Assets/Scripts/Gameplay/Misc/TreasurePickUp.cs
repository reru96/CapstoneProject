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

    public void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var inputManager = ServiceLocator.Get<InputManager>(); 
            actionButton.gameObject.SetActive(true);
            TextMeshProUGUI text = actionButton.GetComponentInChildren<TextMeshProUGUI>();
            if (text != null)
                text.text = $"Press {inputManager.config.action}";
            if (Input.GetKeyDown(inputManager.config.action))
            { 
                OpenChest();
            }
        }

    }

    public void OnTriggerExit(Collider other)
    {
        actionButton?.gameObject.SetActive(false);  
    }
    public void OpenChest()
    {
        top.gameObject.transform.Rotate(-90f, 0, 0);
        var items = chestData.GetRandomItems();
        uiManager.ShowChoices(items);
    }
}
