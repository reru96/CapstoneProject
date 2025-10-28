using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;

public class StatueShop : MonoBehaviour
{
    public float interactionRange = 3f;
    public float waitTime = 10f;
    public List<SOShopItem> availableItems;

    private PlayerSpawnManager spawnManager;

    private void Start()
    {
        StartCoroutine(WaitPlayer());
    }

    private IEnumerator WaitPlayer()
    {
        yield return new WaitForSeconds(interactionRange);
        spawnManager = ServiceLocator.Get<PlayerSpawnManager>();

    }
    private void Update()
    {
        if (spawnManager.Player == null) return;

        float distance = Vector3.Distance(transform.position, spawnManager.Player.transform.position);
        if (distance < interactionRange)
        {
            var inputManager = ServiceLocator.Get<InputManager>();
            if (Input.GetKeyDown(inputManager.config.action))
            {
                OpenShop();
            }
        }
    }

    private void OpenShop()
    {
        var playerStats = spawnManager.Player.GetComponent<PlayerStats>();
        if (playerStats == null) return;

        var uiManager = ServiceLocator.Get<GameUIManager>();
        uiManager.ShowShop(availableItems, playerStats, this);
    }

    public void CloseShop()
    {
        var uiManager = ServiceLocator.Get<GameUIManager>();
        uiManager.HideShop();
    }
}
