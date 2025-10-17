using Core;
using System;
using UnityEngine;

namespace Gameplay
{
    public class ClassSelectionManager : Injectable<ClassSelectionManager>
    {
        public SOPlayerClass SelectedClass { get; private set; }

        public event Action<SOPlayerClass> OnClassChanged;
        protected override void Awake()
        {
            base.Awake();
            DontDestroyOnLoad(gameObject);
        }
        public void SetClass(SOPlayerClass playerClass)
        {
            if (playerClass == SelectedClass) return;

            SelectedClass = playerClass;
            OnClassChanged?.Invoke(playerClass);
            Debug.Log($"[ClassSelectionManager] Classe selezionata: {playerClass.name}");
        }
    }
}