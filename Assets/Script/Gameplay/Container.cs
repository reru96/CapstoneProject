using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using UnityEngine;

public class Container : MonoBehaviour
{
    public static ObjectResolver Resolver { get; private set; }

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

        if (logInitialization)
            Debug.Log("Container di Gameplay inizializzato.");
    }
}
