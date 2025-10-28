using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameUIManager : Injectable<GameUIManager>
{
   
    public RestPointUI restPointUI;
    public UITreasure treasureUI;
    public UInventory inventoryUI;
    public UIStatic staticUI;
    public ShopUI shopUI;


    [SerializeField] private CanvasGroup actionPromptGroup;
    [SerializeField] private TMP_Text actionPromptText;

    private bool isInventoryOpen = false;

    protected override void Awake()
    {
        base.Awake();   
    }

    public void OnEnable()
    {
        HideAll();
        HideActionPrompt();

        GameEvent.OnPlayerSpawned += InitializeStaticUIForPlayer;

        UpdateWeaponUI();

    }

    public void OnDislable()
    {
        GameEvent.OnPlayerSpawned -= InitializeStaticUIForPlayer;
    }


    private void InitializeStaticUIForPlayer()
    {
        if (!ServiceLocator.TryGet<PlayerSpawnManager>(out var playerSpawnMgr))
            return;

        var player = playerSpawnMgr.Player;
        if (player == null) return;

        var life = player.GetComponent<LifeController>();
        var mana = player.GetComponent<ManaController>();
        var stamina = player.GetComponent<StaminaController>();
        var exp = player.GetComponent<PlayerStats>();

        staticUI?.Initialize(life, mana, stamina);

        if (exp != null)
            staticUI?.UpdateExp(exp.exp);

        UpdateWeaponUI();
    }

    private void Update()
    {
        HandleInventoryToggle();
        UpdateWeaponUI();
       
    }

    public void UpdateWeaponUI()
    {
        if (ServiceLocator.TryGet<InventoryManager>(out var inventoryManager))
        {
            var currentWeapon = inventoryManager.runInventory?.CycleWeapon(0);
            staticUI?.SetWeapon(currentWeapon);
        }
    }

    private void HandleInventoryToggle()
    {
        var inputManager = ServiceLocator.Get<InputManager>();
        if (Input.GetKeyDown(inputManager.config.pause))
        {
            isInventoryOpen = !isInventoryOpen;

            if (isInventoryOpen)
            {
                ShowInventory();
            }
            else
            {
                HideInventory();
            }
        }
    }

    public void ShowInventory()
    {
        
        HideAllMenusExcept(inventoryUI);
        inventoryUI?.SetInventoryVisibility(true);
    }

    public void HideInventory()
    {
        inventoryUI?.SetInventoryVisibility(false);
        isInventoryOpen = false;
    }

    public void ShowLevelUp()
    {
        HideAllMenusExcept(restPointUI);
        restPointUI?.ShowLevelUpPanel();
    }

    public void HideLevelUp()
    {
        restPointUI?.HideLevelUpPanel();
    }

    public void ShowTreasure(List<SORunItem> items)
    {
        HideAllMenusExcept(treasureUI);
        treasureUI?.ShowChoices(items);
    }

    public void HideTreasure()
    {
        treasureUI?.Hide();
    }

    public void ShowRestPrompt(string message)
    {
        restPointUI?.ShowPrompt(message);
    }

    public void HideRestPrompt()
    {
        restPointUI?.HidePrompt();
    }

    public void ShowStaticUI()
    {
        staticUI?.Show();
    }

    public void HideStaticUI()
    {
        staticUI?.Hide();
    }

    public void ShowActionPrompt(string message)
    {
        if (actionPromptGroup == null || actionPromptText == null)
            return;

        actionPromptText.text = message;
        actionPromptGroup.alpha = 1f;
        actionPromptGroup.interactable = false;
        actionPromptGroup.blocksRaycasts = false;
    }

    public void HideActionPrompt()
    {
        if (actionPromptGroup == null) return;

        actionPromptGroup.alpha = 0f;
        actionPromptGroup.interactable = false;
        actionPromptGroup.blocksRaycasts = false;
    }

    private void HideAllMenusExcept(MonoBehaviour exception)
    {
        if (restPointUI != exception)
            restPointUI?.HideLevelUpPanel();

        if (treasureUI != exception)
            treasureUI?.Hide();

        if (inventoryUI != exception)
            inventoryUI?.SetInventoryVisibility(false);
    }

    public void HideAll()
    {
        restPointUI?.HideLevelUpPanel();
        treasureUI?.Hide();
        inventoryUI?.SetInventoryVisibility(false);
        restPointUI?.HidePrompt();
    }

}


