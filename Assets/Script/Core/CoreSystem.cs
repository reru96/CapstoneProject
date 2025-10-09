using System.Collections;
using System.Collections.Generic;
using DG.Tweening.Core.Easing;
using UnityEngine;

public class CoreSystem : MonoBehaviour
{
    public static CoreSystem Instance { get; private set; }

    public DIContainer Container { get; private set; }
    public ObjectResolver Resolver { get; private set; }

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        Container = new DIContainer();
        Resolver = new ObjectResolver(Container);

        RegisterDefaultDependencies();
    }

    private void RegisterDefaultDependencies()
    {
        Container.Register<CoreSystem>(this);
        Container.Register<DIContainer>(Container);
        Container.Register<ObjectResolver>(Resolver);
    }
}
