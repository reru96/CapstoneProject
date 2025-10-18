using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextScene : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            var inputManager = ServiceLocator.Get<InputManager>();
            gameObject.SetActive(true);
            if (Input.GetKeyDown(inputManager.config.ability_1))
            {
                SceneManager.LoadScene("Level2");
            }
        }
    }
}