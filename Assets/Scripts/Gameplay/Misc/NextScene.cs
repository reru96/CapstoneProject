using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    public string scene = "DungeonScene";
    private bool playerInTrigger = false;
    private GameManager gameManager;
   
    private void Start()
    {
        gameManager = ServiceLocator.Get<GameManager>();
    }
    private void Update()
    {
        var inputManager = ServiceLocator.Get<InputManager>();
        if (playerInTrigger && Input.GetKeyDown(inputManager.config.action))
        {
            gameManager.LoadScene(scene);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = true;
            var inputManager = ServiceLocator.Get<InputManager>();
            var gameUIManager = ServiceLocator.Get<GameUIManager>();
            gameUIManager?.ShowActionPrompt($"Press {inputManager.config.action} to open");
        }


    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            playerInTrigger = false;
            var inputManager = ServiceLocator.Get<InputManager>();
            var gameUIManager = ServiceLocator.Get<GameUIManager>();
            gameUIManager?.HideActionPrompt();
        }
    }
}
