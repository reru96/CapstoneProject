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
    public CanvasGroup canvasGroup;
    public SOShopItem[] items;           
    public Button[] buttons;             
    public TextMeshProUGUI[] labels;     
    public TextMeshProUGUI[] priceLabels;
    public Image[] icons;                

    public TextMeshProUGUI coinsText;

    private PermanentInventory inventory;
    private PlayerStats playerStats;
    private GameManager gameManager;
    private Coroutine[] messageCoroutines;

    private void OnEnable()
    {
        Hide();
        if (gameManager == null)
            gameManager = ServiceLocator.Get<GameManager>();

        CoinManager.Instance.OnCoinsChanged += UpdateCoinsUI;
        UpdateCoinsUI(CoinManager.Instance.GetCoins());
    }

    private void OnDisable()
    {
        CoinManager.Instance.OnCoinsChanged -= UpdateCoinsUI;
    }

    public void Initialize(PermanentInventory inv, PlayerStats stats)
    {
        inventory = inv;
        playerStats = stats;

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

            UpdateButtonState(i);
        }

        UpdateCoinsUI(CoinManager.Instance.GetCoins());
    }

    private void OnBuyItem(int index)
    {
        var item = items[index];

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
        ServiceLocator.Get<InventoryManager>().SavePermanentInventory();
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

    private void UpdateCoinsUI(int amount)
    {
        if (coinsText != null)
            coinsText.text = $"Coins: {amount}";
    }

    public void Hide()
    {
        if(canvasGroup != null)
        {
            canvasGroup.alpha = 0f;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
        }
    }

    public void Show()
    {
        if(canvasGroup != null)
        {
            canvasGroup.alpha = 1f;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
        }
    }
}
