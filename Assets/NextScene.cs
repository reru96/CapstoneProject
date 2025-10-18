using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    public float interactionDistance = 3f; 
    public string nextSceneName = "Level2";

    private Transform playerTransform;
    private InputManager inputManager;

    private void Start()
    {
     
        var spawner = ServiceLocator.Get<PlayerSpawnManager>();
        if (spawner != null && spawner.Player != null)
            playerTransform = spawner.Player.transform;

        inputManager = ServiceLocator.Get<InputManager>();
    }

    private void Update()
    {
        if (playerTransform == null || inputManager == null) return;

        float distance = Vector3.Distance(transform.position, playerTransform.position);
        if (distance <= interactionDistance && Input.GetKeyDown(inputManager.config.ability_1))
        {
            SceneManager.LoadScene(nextSceneName);
        }
    }
}