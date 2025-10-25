using System.Collections;
using System.Collections.Generic;
using TMPro;
using Core;
using Gameplay;
using UnityEngine;
using UnityEngine.UI;

public class UIChoiseSlot : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private TMP_Text nameText;
    [SerializeField] private TMP_Text descriptionText;

    private Button button;
    private SORunItem currentItem;
    private UITreasure treasureUI;

    private void Awake()
    {
        button = GetComponent<Button>();
        button.onClick.RemoveAllListeners();
        button.onClick.AddListener(OnSelect);
    }

    public void Initialize(UITreasure parent)
    {
        treasureUI = parent;
    }

    public void SetChoice(SORunItem item)
    {
        currentItem = item;

        if (icon) icon.sprite = item.icon;
        if (nameText) nameText.text = item.itemName;
        if (descriptionText) descriptionText.text = item.description;
    }

    private void OnSelect()
    {
        if (currentItem == null || treasureUI == null)
            return;

        treasureUI.OnItemChosen(currentItem);
    }
}
