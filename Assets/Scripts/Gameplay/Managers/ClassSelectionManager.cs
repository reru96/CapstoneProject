using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassSelectionManager : Injectable<ClassSelectionManager>
{
    [SerializeField] private SOPlayerClass[] availableClasses;

    public SOPlayerClass SelectedClass { get; private set; }

    public event System.Action<SOPlayerClass> OnClassChanged;

    protected override void OnInjected(ObjectResolver resolver)
    {
        base.OnInjected(resolver);
  
    }

    public void SelectClass(SOPlayerClass playerClass)
    {
        if (playerClass == null) return;
        if (!System.Array.Exists(availableClasses, c => c == playerClass)) return;

        SelectedClass = playerClass;
        OnClassChanged?.Invoke(playerClass);
    }

    public SOPlayerClass[] GetAvailableClasses() => availableClasses;
}