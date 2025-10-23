using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Core;
using Gameplay;
using System.Collections.Generic;

public class RestPointUI : MonoBehaviour
{
    public PlayerStats playerStats;

    public CanvasGroup promptGroup;
    public TextMeshProUGUI promptText;
    public CanvasGroup levelUpGroup;

    public CanvasGroup confirmationPopupGroup;
    public Button confirmYesButton;
    public Button confirmNoButton;

    public TextMeshProUGUI levelText;
    public TextMeshProUGUI expText;
    public TextMeshProUGUI pointsText;

    public TextMeshProUGUI strengthText;
    public TextMeshProUGUI dexterityText;
    public TextMeshProUGUI intelligenceText;
    public TextMeshProUGUI faithText;
    public TextMeshProUGUI arcaneText;
    public TextMeshProUGUI healthText;
    public TextMeshProUGUI manaText;
    public TextMeshProUGUI staminaText;

    public TextMeshProUGUI forceDefenseText;
    public TextMeshProUGUI slashingDefenseText;
    public TextMeshProUGUI piercingDefenseText;
    public TextMeshProUGUI iceDefenseText;
    public TextMeshProUGUI electricityDefenseText;
    public TextMeshProUGUI fireDefenseText;

    public Button addStrengthBtn, removeStrengthBtn;
    public Button addDexterityBtn, removeDexterityBtn;
    public Button addIntelligenceBtn, removeIntelligenceBtn;
    public Button addFaithBtn, removeFaithBtn;
    public Button addArcaneBtn, removeArcaneBtn;
    public Button addHealthBtn, removeHealthBtn;
    public Button addManaBtn, removeManaBtn;
    public Button addStaminaBtn, removeStaminaBtn;

    public Button confirmButton;
    public Button cancelButton;

    private Dictionary<string, int> pendingChanges = new();

    private void Start()
    {
     
        addStrengthBtn.onClick.AddListener(() => ModifyStat("Strength", +1));
        removeStrengthBtn.onClick.AddListener(() => ModifyStat("Strength", -1));

        addDexterityBtn.onClick.AddListener(() => ModifyStat("Dexterity", +1));
        removeDexterityBtn.onClick.AddListener(() => ModifyStat("Dexterity", -1));

        addIntelligenceBtn.onClick.AddListener(() => ModifyStat("Intelligence", +1));
        removeIntelligenceBtn.onClick.AddListener(() => ModifyStat("Intelligence", -1));

        addFaithBtn.onClick.AddListener(() => ModifyStat("Faith", +1));
        removeFaithBtn.onClick.AddListener(() => ModifyStat("Faith", -1));

        addArcaneBtn.onClick.AddListener(() => ModifyStat("Arcane", +1));
        removeArcaneBtn.onClick.AddListener(() => ModifyStat("Arcane", -1));

        addHealthBtn.onClick.AddListener(() => ModifyStat("Health", +1));
        removeHealthBtn.onClick.AddListener(() => ModifyStat("Health", -1));

        addManaBtn.onClick.AddListener(() => ModifyStat("Mana", +1));
        removeManaBtn.onClick.AddListener(() => ModifyStat("Mana", -1));

        addStaminaBtn.onClick.AddListener(() => ModifyStat("Stamina", +1));
        removeStaminaBtn.onClick.AddListener(() => ModifyStat("Stamina", -1));

        confirmButton.onClick.AddListener(OpenConfirmationPopup);
        cancelButton.onClick.AddListener(CancelLevelUp);

        confirmYesButton.onClick.AddListener(ConfirmLevelUp);
        confirmNoButton.onClick.AddListener(CloseConfirmationPopup);

        if (playerStats != null)
            playerStats.StatsUpdated += UpdateUI;

        HidePrompt();
        HideLevelUpPanel();
        HideConfirmationPopup();
    }

    private void OnDestroy()
    {
        if (playerStats != null)
            playerStats.StatsUpdated -= UpdateUI;
    }

    public void ShowPrompt(string message)
    {
        if (promptText != null)
            promptText.text = message;
        SetVisible(promptGroup, true);
    }

    public void HidePrompt()
    {
        SetVisible(promptGroup, false);
    }

    public void ShowLevelUpPanel()
    {
        pendingChanges.Clear();
        SetVisible(levelUpGroup, true);
        UpdateUI();
    }

    public void HideLevelUpPanel()
    {
        SetVisible(levelUpGroup, false);
        pendingChanges.Clear();
    }

    private void OpenConfirmationPopup()
    {
        if (pendingChanges.Count == 0) return;

        SetVisible(confirmationPopupGroup, true); 
    }
    private void CloseConfirmationPopup() => SetVisible(confirmationPopupGroup, false);

    private void HideConfirmationPopup()
    {
        SetVisible(confirmationPopupGroup, false);
    }

    private void ModifyStat(string stat, int delta)
    {
        if (!pendingChanges.ContainsKey(stat))
            pendingChanges[stat] = 0;

        if (delta < 0 && pendingChanges[stat] <= 0)
            return;

        
        int totalSpent = 0;
        foreach (var kvp in pendingChanges)
            totalSpent += kvp.Value;

        if (delta > 0 && totalSpent >= playerStats.StatPoints)
            return;

        pendingChanges[stat] += delta;
        UpdateUI();
    }

    private void ConfirmLevelUp()
    {
        foreach (var kvp in pendingChanges)
        {
            for (int i = 0; i < kvp.Value; i++)
                playerStats.AllocateStatPoint(kvp.Key);
        }

        pendingChanges.Clear();
        CloseConfirmationPopup();
        HideLevelUpPanel();
        playerStats.RecalculateStats();
    }

    private void CancelLevelUp()
    {
        pendingChanges.Clear();
        HideLevelUpPanel();
    }

    private void UpdateUI()
    {
        if (playerStats == null) return;

        int totalSpent = 0;
        foreach (var kvp in pendingChanges)
            totalSpent += kvp.Value;

        int remainingPoints = playerStats.StatPoints - totalSpent;
        if (remainingPoints < 0) remainingPoints = 0;

        levelText.text = $"Level: {playerStats.Level}";
        expText.text = $"Exp: {playerStats.exp} / {playerStats.expToNextLevel}";
        pointsText.text = $"Points left: {remainingPoints}";

        strengthText.text = $"Strength: {playerStats.Strength + GetPending("Strength")}";
        dexterityText.text = $"Dexterity: {playerStats.Dexterity + GetPending("Dexterity")}";
        intelligenceText.text = $"Intelligence: {playerStats.Intelligence + GetPending("Intelligence")}";
        faithText.text = $"Faith: {playerStats.Faith + GetPending("Faith")}";
        arcaneText.text = $"Arcane: {playerStats.Arcane + GetPending("Arcane")}";
        healthText.text = $"Health: {playerStats.Health + GetPending("Health") + 10}";
        manaText.text = $"Mana: {playerStats.Mana + GetPending("Mana") + 5}";
        staminaText.text = $"Stamina: {playerStats.Stamina + GetPending("Stamina") + 5}";

        forceDefenseText.text = $"Force Defense: {playerStats.Defenses[DamageType.Force]}";
        slashingDefenseText.text = $"Slashing Defense: {playerStats.Defenses[DamageType.Slashing]}";
        piercingDefenseText.text = $"Piercing Defense: {playerStats.Defenses[DamageType.Piercing]}";
        iceDefenseText.text = $"Ice Defense: {playerStats.Defenses[DamageType.Ice]}";
        electricityDefenseText.text = $"Electricity Defense: {playerStats.Defenses[DamageType.Electricity]}";
        fireDefenseText.text = $"Fire Defense: {playerStats.Defenses[DamageType.Fire]}";
    }
    private int GetPending(string stat) => pendingChanges.ContainsKey(stat) ? pendingChanges[stat] : 0;

    private void SetVisible(CanvasGroup group, bool visible)
    {
        if (group == null) return;
        group.alpha = visible ? 1 : 0;
        group.interactable = visible;
        group.blocksRaycasts = visible;
    }
}
