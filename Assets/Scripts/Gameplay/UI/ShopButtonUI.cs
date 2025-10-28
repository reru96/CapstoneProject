using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopButtonUI : MonoBehaviour
{
    public SOShopItem item;
    public Button button;
    public TextMeshProUGUI label;
    public TextMeshProUGUI priceLabel;
    public Image icon;

    private System.Action<ShopButtonUI> onBuy;
    private Coroutine messageCoroutine;

    public void Initialize(System.Action<ShopButtonUI> onBuy)
    {
        this.onBuy = onBuy;
        if (item != null)
        {
            label.text = $"{item.itemName} - {item.description}";
            priceLabel.text = $"{item.cost} coins";
            if (icon != null) icon.sprite = item.icon;
        }

        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(() => onBuy(this));
    }

    public void UpdateState(PermanentInventory inventory)
    {
        if (item == null || inventory == null) return;

        if (inventory.unlockedUpgrades.Contains(item))
            SetSoldState();
        else
            SetAvailableState();
    }

    public void SetSoldState()
    {
        label.text = "Sold";
        priceLabel.text = "";
        button.interactable = false;
    }

    public void SetAvailableState()
    {
        if (item == null) return;
        label.text = item.itemName;
        priceLabel.text = $"{item.cost} coins";
        button.interactable = true;
    }

    public void ShowMessage(string message)
    {
        if (messageCoroutine != null)
            StopCoroutine(messageCoroutine);

        messageCoroutine = StartCoroutine(TemporaryMessage(message));
    }
    private IEnumerator TemporaryMessage(string msg)
    {
        string original = label.text;
        label.text = msg;
        yield return new WaitForSeconds(1.5f);
        label.text = original;
    }
}
