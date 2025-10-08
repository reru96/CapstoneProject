using System;
using System.Collections.Generic;
using UnityEngine;


public class ObjectResolver
{
  
    private readonly Dictionary<Type, object> _instancePerTypeMap = new Dictionary<Type, object>();

    public void RegisterInstance<T>(T instance)
    {
        var type = typeof(T);
        _instancePerTypeMap[type] = instance ?? throw new ArgumentNullException(nameof(instance));
    }

    public void UnregisterInstance<T>()
    {
        var type = typeof(T);
        _instancePerTypeMap.Remove(type);
    }


    public T Resolve<T>()
    {
        var type = typeof(T);
        if (_instancePerTypeMap.TryGetValue(type, out var instance))
        {
            return (T)instance;
        }

        Debug.LogError($"Non ho potuto risolvere il tipo {type}");
        return default;
    }

    public bool IsRegistered<T>()
    {
        return _instancePerTypeMap.ContainsKey(typeof(T));
    }
}