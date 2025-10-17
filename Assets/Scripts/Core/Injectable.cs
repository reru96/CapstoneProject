using UnityEngine;

namespace Core
{
    public abstract class Injectable<T> : MonoBehaviour where T : class
    {
        protected virtual void Awake()
        {
            ServiceLocator.Register<T>(this as T);
        }

        protected virtual void OnDestroy()
        {
            ServiceLocator.Unregister<T>();
        }
    }
}