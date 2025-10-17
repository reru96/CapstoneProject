using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Core;
using Gameplay;

public class CharacterSelectionUI : MonoBehaviour
{
    [SerializeField] private SOPlayerClass playerClass;
    [SerializeField] private string nextSceneName = "Level1";

    public void OnClickSelect()
    {
        if (playerClass == null || playerClass.prefab == null)
        {
            Debug.LogWarning("[CharacterSelectionButton] Classe o prefab non assegnato!");
            return;
        }

        var classMgr = ServiceLocator.Get<ClassSelectionManager>();
        classMgr.SetClass(playerClass);
        Debug.Log($"[CharacterSelectionButton] Classe selezionata: {playerClass.className}");

        SceneManager.LoadScene(nextSceneName);
    }
}

