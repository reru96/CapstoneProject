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

    private LifeController lifeController;
    private ManaController manaController;
    private StaminaController staminaController;
    private PlayerStats playerStats;
    private GameManager gameManager;

    private void Awake()
    {
        SetVisible(true);
        ServiceLocator.TryGet(out gameManager);

        if (gameManager != null)
            GameManager.OnCoinsChanged += UpdateCurrency;
    }

    public void Initialize(LifeController life, ManaController mana, StaminaController stamina, PlayerStats stats)
    {
        lifeController = life;
        manaController = mana;
        staminaController = stamina;

        if (stats != null)
        {
            if (playerStats != null)
            {
                playerStats.ExpChanged -= UpdateExp; 
            }

            playerStats = stats;
            playerStats.ExpChanged += UpdateExp;
        }

        UpdateBars();
        UpdateExp(playerStats?.exp ?? 0);
        if (gameManager != null)
            UpdateCurrency(gameManager.Coins);
    }

    private void OnDisable()
    {
        if (playerStats != null)
            playerStats.ExpChanged -= UpdateExp;

        if (gameManager != null)
            GameManager.OnCoinsChanged -= UpdateCurrency;
    }

    private void Update()
    {
        UpdateBars();
    }

    private void UpdateBars()
    {
        if (lifeController != null && healthBar != null)
        {
            float hp = lifeController.GetHp();
            float maxHp = Mathf.Max(1f, lifeController.GetMaxHp());
            healthBar.value = Mathf.Clamp01(hp / maxHp);
        }

        if (manaController != null && manaBar != null)
        {
            float cur = manaController.currentMana;
            float max = Mathf.Max(1f, manaController.MaxMana);
            manaBar.value = Mathf.Clamp01(cur / max);
        }

        if (staminaController != null && staminaBar != null)
        {
            float cur = staminaController.currentStamina;
            float max = Mathf.Max(1f, staminaController.maxStamina);
            staminaBar.value = Mathf.Clamp01(cur / max);
        }
    }

    public void UpdateCurrency(int amount)
    {
        if (currencyText != null)
            currencyText.text = $"Coin: {amount}";
    }

    public void UpdateExp(int currentExp)
    {
        if (expText != null && playerStats != null)
            expText.text = $"Exp: {currentExp}";
    }

    public void SetWeapon(SOWeapon weapon)
    {
        if (weaponSlotImage == null) return;

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

    public void Show() => SetVisible(true);
    public void Hide() => SetVisible(false);

    private void SetVisible(bool visible)
    {
        CanvasGroup cg = GetComponent<CanvasGroup>();
        if (cg != null)
        {
            cg.alpha = visible ? 1f : 0f;
            cg.interactable = visible;
            cg.blocksRaycasts = visible;
        }
    }
}


