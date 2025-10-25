using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UITreasure : MonoBehaviour
{
    [SerializeField] private UIChoiseSlot[] choiceSlots;
    public CanvasGroup canvasGroup;

    private List<SORunItem> currentChoices;

    private void Start()
    {
        foreach (var slot in choiceSlots)
        {
            slot.Initialize(this);
            slot.gameObject.SetActive(false);
        }

        Hide();
    }

    public void ShowChoices(List<SORunItem> items)
    {
        currentChoices = items;

        for (int i = 0; i < choiceSlots.Length; i++)
        {
            if (i < items.Count)
            {
                choiceSlots[i].gameObject.SetActive(true);
                choiceSlots[i].SetChoice(items[i]);
            }
            else
            {
                choiceSlots[i].gameObject.SetActive(false);
            }
        }

        Show();
    }

    public void OnItemChosen(SORunItem chosen)
    {
        var inventoryManager = ServiceLocator.Get<InventoryManager>();
        inventoryManager.runInventory.AddItem(chosen);

        currentChoices?.Clear();

        foreach (var slot in choiceSlots)
        {
            slot.gameObject.SetActive(false);
        }

        Hide();
    }

    public void Show()
    {
        if (canvasGroup == null)
            return;

        canvasGroup.alpha = 1f;
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
    }

    public void Hide()
    {
        if (canvasGroup == null)
            return;

        canvasGroup.alpha = 0f;
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
    }
}
