using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputRelocationUI : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform container;

    private InputManager inputManager;

    private IEnumerator Start()
    {
        inputManager = CoreSystem.Instance.Container.Resolve<InputManager>();

        if (inputManager == null)
        {
            Debug.LogError("[InputRelocationUI] Impossibile trovare InputManager!");
            yield break;
        }

        BuildUI();
    }

    private void BuildUI()
    {
        foreach (Transform child in container)
            Destroy(child.gameObject);

        var fields = typeof(MyInput).GetFields();

        foreach (var field in fields)
        {
            string actionName = field.Name;
            KeyCode currentKey = (KeyCode)field.GetValue(inputManager.Config);

            GameObject newButton = Instantiate(buttonPrefab, container);
            TMP_Text label = newButton.GetComponentInChildren<TMP_Text>();
            Button button = newButton.GetComponent<Button>();

            label.text = $"{actionName}: {currentKey}";

            button.onClick.AddListener(() =>
            {
                StartCoroutine(WaitForKey(actionName, label));
            });
        }
    }

    private IEnumerator WaitForKey(string actionName, TMP_Text label)
    {
        label.text = $"{actionName}: Premi un tasto...";

        bool keySet = false;
        while (!keySet)
        {
            foreach (KeyCode k in Enum.GetValues(typeof(KeyCode)))
            {
                if (Input.GetKeyDown(k))
                {
                    inputManager.SetKey(actionName, k);
                    label.text = $"{actionName}: {k}";
                    keySet = true;
                    break;
                }
            }
            yield return null;
        }
    }
}
