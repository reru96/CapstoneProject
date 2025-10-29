using System.Collections;
using System.Collections.Generic;
using Core;
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
        Show(loseMenu);
        var audioManager = ServiceLocator.TryGet<AudioManager>();
        audioManager.PlaySfx(deathMusic);
    }
    private void ShowWinMenu()
    {
        Show(winMenu);
        var audioManager = ServiceLocator.TryGet<AudioManager>();
        audioManager.PlaySfx(winMusic);
        
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
