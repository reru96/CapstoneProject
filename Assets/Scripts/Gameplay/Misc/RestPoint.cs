using System.Collections;
using System.Collections.Generic;
using Codice.Client.Common.GameUI;
using Core;
using Gameplay;
using UnityEngine;

public class RestPoint : MonoBehaviour
{
    public Transform newSpawnPoint;
    private InputManager inputManager;
    private GameUIManager gameUIManager;
    private bool playerInRange = false;

    private void Start()
    {
        inputManager = ServiceLocator.Get<InputManager>();
        gameUIManager = ServiceLocator.Get<GameUIManager>();
    }

    private void Update()
    {
        if (!playerInRange)
            return;

        if (Input.GetKeyDown(inputManager.config.action))
        {
            var player = ServiceLocator.Get<PlayerSpawnManager>().Player;
            if (player == null) return;

            var stats = player.GetComponent<PlayerStats>();
            if (stats == null) return;

            var life = player.GetComponent<LifeController>();
            if (life == null) return;

            life.SetHp(life.GetMaxHp());

            var spawnManager = ServiceLocator.Get<PlayerSpawnManager>();
            spawnManager.SetRespawnPoint(newSpawnPoint);

            gameUIManager.HideRestPrompt();
            gameUIManager.ShowLevelUp();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = true;
            gameUIManager.ShowRestPrompt($"Press {inputManager.config.action} to rest");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInRange = false;
            gameUIManager.HideRestPrompt();
            gameUIManager.HideLevelUp();
        }
    }
}
