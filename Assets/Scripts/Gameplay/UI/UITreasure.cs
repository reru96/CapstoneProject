using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITreasure : MonoBehaviour
{
    public GameObject choiceButtonPrefab;
    public Transform choiceContainer;

    public CanvasGroup canvasGroup;

    private List<SORunItem> currentChoices;

    private void Start()
    {
        Hide(); 
    }

    public void ShowChoices(List<SORunItem> items)
    {
        currentChoices = items;

        foreach (Transform child in choiceContainer)
            Destroy(child.gameObject);

        foreach (var item in items)
        {
            GameObject buttonObj = Instantiate(choiceButtonPrefab, choiceContainer);
            Button button = buttonObj.GetComponent<Button>();
            TextMeshProUGUI text = buttonObj.GetComponentInChildren<TextMeshProUGUI>();
            Image icon = buttonObj.GetComponentInChildren<Image>();

            if (text != null)
                text.text = item.itemName;

            if (icon != null)
                icon.sprite = item.icon;

            button.onClick.AddListener(() => OnItemChosen(item));
        }

        Show(); 
    }

    private void OnItemChosen(SORunItem chosen)
    {
    
        var inventoryManager = ServiceLocator.Get<InventoryManager>();
        inventoryManager.runInventory.AddItem(chosen);

        Hide(); 
    }

    public void Show()
    {
        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        if (canvasGroup == null)
        {
            return;
        }

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
