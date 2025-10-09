using System;
using System.Reflection;

public class ObjectResolver
{
    private readonly DIContainer _container;

    public ObjectResolver(DIContainer container)
    {
        _container = container;
    }

    public void Resolve(object target)
    {
  
        var type = target.GetType();
        var members = type.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);

        foreach (var member in members)
        {
            var injectAttr = member.GetCustomAttribute<InjectAttribute>();
            if (injectAttr == null) continue;

            var memberType = member is FieldInfo f ? f.FieldType :
                             member is PropertyInfo p ? p.PropertyType : null;

            if (memberType == null) continue;

            var dependency = _container.Resolve(memberType);
            if (member is FieldInfo field)
                field.SetValue(target, dependency);
            else if (member is PropertyInfo prop)
                prop.SetValue(target, dependency);
        }

     
        if (target is IInject injectable)
            injectable.InjectDependencies();
    }
}