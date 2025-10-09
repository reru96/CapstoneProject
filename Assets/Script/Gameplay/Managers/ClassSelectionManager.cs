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
        LoadClassFromSettings();
    }

    private void LoadClassFromSettings()
    {
        if (GameSettings.SelectedClass != null)
        {
            SelectClass(GameSettings.SelectedClass);
        }
        else if (availableClasses.Length > 0)
        {
            SelectClass(availableClasses[0]);
        }
    }

    public void SelectClass(SOPlayerClass playerClass)
    {
        if (playerClass == null) return;
        if (!System.Array.Exists(availableClasses, c => c == playerClass)) return;

        SelectedClass = playerClass;
        GameSettings.SelectedClass = playerClass;
        OnClassChanged?.Invoke(playerClass);
    }

    public SOPlayerClass[] GetAvailableClasses() => availableClasses;
}