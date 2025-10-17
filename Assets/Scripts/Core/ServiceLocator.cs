using System;
using System.Collections.Generic;

namespace Core
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new();

        public static void Register<T>(T service)
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
                _services[type] = service;
            else
                _services.Add(type, service);
        }

        public static void Unregister<T>()
        {
            _services.Remove(typeof(T));
        }

        public static T Get<T>()
        {
            if (_services.TryGetValue(typeof(T), out var service))
                return (T)service;

            throw new Exception($"[ServiceLocator] Service {typeof(T).Name} not found.");
        }

        public static bool TryGet<T>(out T service)
        {
            if (_services.TryGetValue(typeof(T), out var result))
            {
                service = (T)result;
                return true;
            }

            service = default;
            return false;
        }

        public static void Clear()
        {
            _services.Clear();
        }
        public static bool Has<T>()
        {
            return _services.ContainsKey(typeof(T));
        }

    }
}
