using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoreSystem : MonoBehaviour
{
    public static ObjectResolver Resolver { get; private set; }

    private void Awake()
    {
        if (Resolver == null)
            Resolver = new ObjectResolver();

        DontDestroyOnLoad(gameObject);
    }
}
