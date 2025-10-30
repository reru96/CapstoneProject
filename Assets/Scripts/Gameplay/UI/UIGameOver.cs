using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;
using UnityEngine.SceneManagement;

public class UIGameOver : MonoBehaviour
{
    public CanvasGroup loseMenu;
    public CanvasGroup winMenu;

    public string deathMusic;
    public string winMusic;

    private void OnEnable()
    {
        GameEvent.OnPlayerDead += ShowLoseMenu;
        GameEvent.OnBossDead += ShowWinMenu;
    }

    private void OnDisable()
    {
        GameEvent.OnPlayerDead -= ShowLoseMenu;
        GameEvent.OnBossDead -= ShowWinMenu;
    }

    private void Start()
    {
        Hide(loseMenu);
        Hide(winMenu);
    }

    private void ShowLoseMenu()
    {   
        var audioManager = ServiceLocator.Get<AudioManager>();
        audioManager.PlaySfx(deathMusic);
        Show(loseMenu);
        StartCoroutine(RestartLevel());

    }
    private IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(1f); 
        SceneManager.LoadScene("Level1");
       
    }

    private void ShowWinMenu()
    {
        Show(winMenu);
        var audioManager = ServiceLocator.Get<AudioManager>();
        audioManager.PlaySfx(winMusic);
        StartCoroutine(RestartLevel());
    }

    public void Show(CanvasGroup group)
    {
        group.alpha = 1;
        group.blocksRaycasts = true;
        group.interactable = true;
    }

    public void Hide(CanvasGroup group)
    {
        group.alpha = 0;
        group.blocksRaycasts = false;
        group.interactable = false;
    }

}
