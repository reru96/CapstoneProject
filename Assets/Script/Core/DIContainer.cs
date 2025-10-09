using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DIContainer 
{
    private readonly Dictionary<Type, object> _registrations = new();


    public void Register<T>(T instance)
    {
        _registrations[typeof(T)] = instance;
    }

    public T Resolve<T>()
    {
        if (_registrations.TryGetValue(typeof(T), out var instance))
            return (T)instance;

        throw new Exception($"[DIContainer] Nessuna istanza trovata per tipo {typeof(T)}");
    }

    
    public object Resolve(Type type)
    {
        if (_registrations.TryGetValue(type, out var instance))
            return instance;

        throw new Exception($"[DIContainer] Nessuna istanza trovata per tipo {type}");
    }

}