using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
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
        if (playerInTrigger && Input.GetKeyDown(inputManager.config.ability_1))
        {
            gameManager.LoadScene(scene);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInTrigger = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInTrigger = false;
    }
}
