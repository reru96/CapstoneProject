using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class CharactherSelectionUI : MonoBehaviour
{
    [Header("Personaggi")]
    [SerializeField] private GameObject[] playerPrefabs;

    [Header("Pulsanti")]
    [SerializeField] private Button[] selectionButtons;

    [Header("Scena di gioco")]
    [SerializeField] private string gameSceneName = "Level1";


    private void Start()
    {
        if (playerPrefabs.Length != selectionButtons.Length)
            Debug.LogWarning("Il numero di pulsanti non corrisponde al numero di prefabs!");

        for (int i = 0; i < selectionButtons.Length; i++)
        {
            int index = i;

            if (selectionButtons[i] == null)
            {
                Debug.LogError("Pulsante " + i + " non assegnato nell'Inspector!");
                continue;
            }

            selectionButtons[i].onClick.AddListener(() => SelectCharacter(index));
        }

 
    }

    private void SelectCharacter(int index)
    {
        var respawnManager = Container.Resolver.Resolve<RespawnManager>();
        Debug.Log("Bottone premuto: " + index);

        if (index < 0 || index >= playerPrefabs.Length)
        {
            Debug.LogError("Indice personaggio non valido: " + index);
            return;
        }

        if (playerPrefabs[index] == null)
        {
            Debug.LogError("Prefab del personaggio " + index + " non assegnato!");
            return;
        }

       
        if (respawnManager != null)
        {
            respawnManager.SetPlayerPrefab(playerPrefabs[index]);
            Debug.Log("Prefab impostato correttamente nel RespawnManager.");
        }
        else
        {
            Debug.LogError("RespawnManager non trovato in scena!");
            return;
        }

       
        SceneManager.LoadScene(gameSceneName);
    }

 
}

