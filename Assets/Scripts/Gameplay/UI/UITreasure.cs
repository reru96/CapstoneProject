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
    public GameObject panel;

    private List<SORunItem> currentChoices;

    public void Start()
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

        if (panel != null)
            panel.SetActive(true);
        else
            gameObject.SetActive(true);
    }

    void OnItemChosen(SORunItem chosen)
    {
        Debug.Log($"Hai scelto: {chosen.itemName}");

        var inventoryManager = ServiceLocator.Get<InventoryManager>();
        inventoryManager.runInventory.AddItem(chosen);

        Hide();
    }

    public void Hide()
    {
        if (panel != null)
            panel.SetActive(false);
        else
            gameObject.SetActive(false);
    }
}
