using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using UnityEngine;

public class Container : MonoBehaviour
{
    public static ObjectResolver Resolver { get; private set; }
    [SerializeField] private GameManager gameManager;
    [SerializeField] private RespawnManager respawnManager;
    [SerializeField] private InputManager inputManager;
    [SerializeField] private AudioManager audioManager;

    [Header("Debug")]
    [SerializeField] private bool logInitialization = false;

    private void Awake()
    {
        if (Resolver != null)
        {
            if (logInitialization)
                Debug.LogWarning("Container già inizializzato, distruggo il duplicato.");

            Destroy(gameObject);
            return;
        }

     
        Resolver = new ObjectResolver();

    
        if (CoreSystem.Resolver != null)
        {
            Resolver.RegisterInstance(CoreSystem.Resolver);
        }
        if (gameManager != null) Resolver.RegisterInstance(gameManager);
        if (respawnManager != null) Resolver.RegisterInstance(respawnManager);
        if (inputManager != null) Resolver.RegisterInstance(inputManager);
        if (audioManager != null) Resolver.RegisterInstance(audioManager);

        if (logInitialization)
            Debug.Log("Container di Gameplay inizializzato.");
    }
}
