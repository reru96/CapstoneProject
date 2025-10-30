using System.Collections;
using System.Collections.Generic;
using Core;
using DG.Tweening.Core.Easing;
using Gameplay;
using TMPro;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.UI;

public class ShopUI : MonoBehaviour
{
    [Header("UI References")]
    public CanvasGroup canvasGroup;
    public SOShopItem[] items;
    public Button[] buttons;
    public TextMeshProUGUI[] labels;
    public TextMeshProUGUI[] priceLabels;
    public Image[] icons;
    public TextMeshProUGUI coinsText;

    private PermanentInventory inventory;
    private PlayerStats playerStats;
    private Coroutine[] messageCoroutines;

    private InventoryManager inventoryManager;

    private void OnEnable()
    {
        Hide();

        inventoryManager = ServiceLocator.Get<InventoryManager>();
        if (inventoryManager == null)
        {
            Debug.LogError("[ShopUI] InventoryManager non trovato!");
            return;
        }

        inventory = inventoryManager.permanentInventory;

        inventoryManager.OnInventoryChanged += RefreshShop;
        CoinManager.Instance.OnCoinsChanged += UpdateCoinsUI;

        UpdateCoinsUI(CoinManager.Instance.GetCoins());
        InitializeShop();
    }

    private void OnDisable()
    {
        if (inventoryManager != null)
            inventoryManager.OnInventoryChanged -= RefreshShop;

        if (CoinManager.Instance != null)
            CoinManager.Instance.OnCoinsChanged -= UpdateCoinsUI;
    }

    private void InitializeShop()
    {
        int count = Mathf.Min(items.Length, buttons.Length);
        messageCoroutines = new Coroutine[count];

        for (int i = 0; i < count; i++)
        {
            int index = i;

            if (buttons[i] != null)
            {
                buttons[i].onClick.RemoveAllListeners();
                buttons[i].onClick.AddListener(() => OnBuyItem(index));
            }

            if (icons[i] != null && items[i] != null && items[i].icon != null)
                icons[i].sprite = items[i].icon;

            UpdateButtonState(index);
        }
    }

    private void OnBuyItem(int index)
    {
        var item = items[index];
        if (item == null || inventory == null) return;

        if (inventory.unlockedUpgrades.Contains(item))
        {
            SetSoldState(index);
            return;
        }

        if (!CoinManager.Instance.SpendCoins(item.cost))
        {
            ShowMessage(index, "Not enough coins");
            return;
        }

        inventory.UnlockUpgrade(item);
        inventoryManager.SavePermanentInventory();

        item.Apply(playerStats);

        SetSoldState(index);
        UpdateCoinsUI(CoinManager.Instance.GetCoins());
    }

    private void UpdateButtonState(int index)
    {
        if (items[index] == null || inventory == null) return;

        if (inventory.unlockedUpgrades.Contains(items[index]))
            SetSoldState(index);
        else
            SetAvailableState(index);
    }

    private void SetSoldState(int index)
    {
        if (labels[index] != null) labels[index].text = "Sold";
        if (priceLabels[index] != null) priceLabels[index].text = "";
        if (buttons[index] != null) buttons[index].interactable = false;
    }

    private void SetAvailableState(int index)
    {
        if (items[index] == null) return;
        if (labels[index] != null) labels[index].text = $"{items[index].itemName} - {items[index].description}";
        if (priceLabels[index] != null) priceLabels[index].text = $"{items[index].cost} coins";
        if (buttons[index] != null) buttons[index].interactable = true;
    }

    private void ShowMessage(int index, string message)
    {
        if (messageCoroutines[index] != null)
            StopCoroutine(messageCoroutines[index]);

        messageCoroutines[index] = StartCoroutine(TemporaryMessage(index, message));
    }

    private IEnumerator TemporaryMessage(int index, string msg)
    {
        string original = labels[index].text;
        labels[index].text = msg;
        yield return new WaitForSeconds(1.5f);
        labels[index].text = original;
        messageCoroutines[index] = null;
    }

    private void RefreshShop()
    {
        for (int i = 0; i < items.Length; i++)
            UpdateButtonState(i);
    }

    private void UpdateCoinsUI(int amount)
    {
        if (coinsText != null)
            coinsText.text = $"Coins: {amount}";
    }

    public void Show()
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = 1f;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.interactable = true;
        UpdateCoinsUI(CoinManager.Instance.GetCoins());
    }

    public void Hide()
    {
        if (canvasGroup == null) return;
        canvasGroup.alpha = 0f;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.interactable = false;
    }

    public void SetPlayerStats(PlayerStats stats)
    {
        playerStats = stats;
    }
}
