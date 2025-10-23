using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Core;
using Gameplay;
using System.Collections.Generic;

public class UIStatic : MonoBehaviour
{
    public Slider healthBar;
    public Slider manaBar;
    public Slider staminaBar;

    public TextMeshProUGUI currencyText;
    public TextMeshProUGUI expText;

    public Image weaponSlotImage;

    public LifeController lifeController;
    public ManaController manaController;
    public StaminaController staminaController;

    private void Awake()
    {
        SetVisible(true); 
    }

    public void Initialize(LifeController life, ManaController mana, StaminaController stamina)
    {
        lifeController = life;
        manaController = mana;
        staminaController = stamina;
    }

    private void Update()
    {
        UpdateBars();
    }

    private void UpdateBars()
    {
        if (lifeController != null && healthBar != null)
            healthBar.value = lifeController.GetHp() / lifeController.GetMaxHp();

        if (manaController != null && manaBar != null)
            manaBar.value = manaController.currentMana / manaController.MaxMana;

        if (staminaController != null && staminaBar != null)
            staminaBar.value = staminaController.currentStamina / staminaController.maxStamina;
    }

    public void SetWeapon(SOWeapon weapon)
    {
        if (weapon != null)
        {
            weaponSlotImage.sprite = weapon.icon;
            weaponSlotImage.enabled = true;
        }
        else
        {
            weaponSlotImage.enabled = false;
        }
    }

    public void UpdateCurrency(int amount)
    {
        if (currencyText != null)
            currencyText.text = $"Coin: {amount}";
    }

    public void UpdateExp(int current)
    {
        if (expText != null)
            expText.text = $"Exp: {current}";
    }

    public void Show() => SetVisible(true);
    public void Hide() => SetVisible(false);

    private void SetVisible(bool visible)
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = visible ? 1 : 0;
            cg.interactable = visible;
            cg.blocksRaycasts = visible;
        }
    }
}


