using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Injectable<T> : MonoBehaviour, IInject where T : MonoBehaviour
{
    protected virtual void Awake()
    {
        DontDestroyOnLoad(this.gameObject);
        CoreSystem.Instance.Container.Register<T>(this as T);
        CoreSystem.Instance.Resolver.Resolve(this);
        OnInjected(CoreSystem.Instance.Resolver);
    }

    protected virtual void OnInjected(ObjectResolver resolver) { }

    protected virtual void OnDestroy()
    {
        
    }

    public virtual void InjectDependencies() { }

    protected bool TryResolve<TDep>(out TDep dependency) where TDep : class
    {
        dependency = CoreSystem.Instance.Container.Resolve<TDep>();
        return dependency != null;
    }

    protected TDep Resolve<TDep>() where TDep : class =>
        CoreSystem.Instance.Container.Resolve<TDep>();
}