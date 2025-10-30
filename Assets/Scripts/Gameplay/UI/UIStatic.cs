using System.Collections.Generic;
using Core;
using Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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

    public Button backToStartMenuButton;

    public GameObject confirmationPrompt; 
    public Button yesButton;
    public Button noButton;
    public TextMeshProUGUI promptText;

    private void Awake()
    {
        SetVisible(true);
        ServiceLocator.TryGet(out gameManager);

        if (gameManager != null)
            CoinManager.Instance.OnCoinsChanged += UpdateCurrency;

        if (backToStartMenuButton != null)
            backToStartMenuButton.onClick.AddListener(ShowConfirmationPrompt);

        if (yesButton != null)
            yesButton.onClick.AddListener(ConfirmReturnToStartMenu);
        if (noButton != null)
            noButton.onClick.AddListener(HideConfirmationPrompt);

        if (confirmationPrompt != null)
            confirmationPrompt.SetActive(false);
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
            UpdateCurrency(CoinManager.Instance.GetCoins());
    }

    private void OnDisable()
    {
        if (backToStartMenuButton != null)
            backToStartMenuButton.onClick.RemoveListener(ShowConfirmationPrompt);
        if (yesButton != null)
            yesButton.onClick.RemoveListener(ConfirmReturnToStartMenu);
        if (noButton != null)
            noButton.onClick.RemoveListener(HideConfirmationPrompt);

        if (playerStats != null)
            playerStats.ExpChanged -= UpdateExp;

        if (gameManager != null)
            CoinManager.Instance.OnCoinsChanged -= UpdateCurrency;
    }

    private void Update()
    {
        UpdateBars();
    }
   
    private void UpdateBars()
    {
        if (lifeController != null && healthBar != null)
        {
            healthBar.maxValue = lifeController.GetMaxHp();
            healthBar.value = lifeController.GetHp();
        }

        if (manaController != null && manaBar != null)
        {
            manaBar.maxValue = manaController.MaxMana;
            manaBar.value = manaController.currentMana;
        }

        if (staminaController != null && staminaBar != null)
        {
            staminaBar.maxValue = staminaController.maxStamina;
            staminaBar.value = staminaController.currentStamina;
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

    private void ShowConfirmationPrompt()
    {
        if (confirmationPrompt != null)
        {
            promptText.text = "Do you want to return to the Start Menu?";
            confirmationPrompt.SetActive(true);
        }
    }

    private void HideConfirmationPrompt()
    {
        if (confirmationPrompt != null)
            confirmationPrompt.SetActive(false);
    }

    private void ConfirmReturnToStartMenu()
    {
        GameController.Instance?.SaveGame();
        SceneManager.LoadScene("StartMenu");
    }
}


