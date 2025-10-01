using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartMenu : MonoBehaviour
{
    public GameObject settingsMenu;
    public GameObject VolumePanel;
    public GameObject InputPanel;

    private void Start()
    {
        settingsMenu.SetActive(false);
    }
    public void StartGame()
    { 
        
        SceneManager.LoadScene(1);
      
    }

    public void ShowOptions()
    {
        settingsMenu.SetActive(true);
    }

    public void HideOptions()
    {
        settingsMenu.SetActive(false);
    }

    public void ShowInputPanel()
    { 
        InputPanel.SetActive(true);
        VolumePanel.SetActive(false);
    }
    public void ShowVolumePanel()
    {  
        VolumePanel.SetActive(true);
        InputPanel.SetActive(false);
    }

    public void OnExitGame()
    {
        Application.Quit();
    }
}
