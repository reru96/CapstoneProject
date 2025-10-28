using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;

public class PoolConfigurator : MonoBehaviour
{
    private ObjectPooler pooler;
    private ClassSelectionManager classManager;

    private void Awake()
    {
        pooler = ServiceLocator.Get<ObjectPooler>();
        classManager = ServiceLocator.Get<ClassSelectionManager>();

        if (classManager != null)
            classManager.OnClassChanged += OnClassChanged;


        if (classManager != null && classManager.SelectedClass != null && pooler != null)
            pooler.ConfigurePoolsForClass(classManager.SelectedClass);
    }

    private void OnDestroy()
    {
        if (classManager != null)
            classManager.OnClassChanged -= OnClassChanged;
    }

    private void OnClassChanged(SOPlayerClass playerClass)
    {
        if (pooler == null || playerClass == null) return;
        pooler.ConfigurePoolsForClass(playerClass);
        Debug.Log($"[PoolConfigurator] Pool configurato per classe {playerClass.className}");
    }
}
