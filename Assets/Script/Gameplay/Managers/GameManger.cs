using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DefaultExecutionOrder(-50)] 
public class GameManager : MonoBehaviour
{
    public static GameState CurrentState { get; private set; } = GameState.None;

   
    public static event Action<GameState> OnGameStateChanged;
    public static event Action OnGameStarted;
    public static event Action OnGamePaused;
    public static event Action OnGameResumed;
    public static event Action OnGameOver;

    private InputManager inputManager;

    private void Awake()
    {
        
        if (Container.Resolver != null)
            Container.Resolver.RegisterInstance(this);
        else
            Debug.LogWarning("[GameManager] Container.Resolver non trovato. Assicurati che il Container sia inizializzato prima.");

        inputManager = Container.Resolver?.Resolve<InputManager>();

        ChangeState(GameState.Loading);
    }

    private void Start()
    {
        
        Invoke(nameof(StartGame), 0.5f);
    }

    private void OnDestroy()
    {
        if (Container.Resolver != null)
            Container.Resolver.UnregisterInstance<GameManager>();
    }

    private void StartGame()
    {
        ChangeState(GameState.Playing);
        OnGameStarted?.Invoke();
    }

    public void ChangeState(GameState newState)
    {
        if (newState == CurrentState)
            return;

        GameState oldState = CurrentState;
        CurrentState = newState;

        Debug.Log($"[GameManager] Cambio stato: {oldState} → {newState}");
        OnGameStateChanged?.Invoke(newState);

        switch (newState)
        {
            case GameState.Playing:
                Time.timeScale = 1f;
                if (oldState == GameState.Paused)
                    OnGameResumed?.Invoke();
                break;

            case GameState.Paused:
                Time.timeScale = 0f;
                OnGamePaused?.Invoke();
                break;

            case GameState.GameOver:
                Time.timeScale = 0f;
                OnGameOver?.Invoke();
                break;
        }
    }

    public void TogglePause()
    {
        if (CurrentState == GameState.Playing)
            ChangeState(GameState.Paused);
        else if (CurrentState == GameState.Paused)
            ChangeState(GameState.Playing);
    }

    public void QuitGame()
    {
        Debug.Log("[GameManager] Quit game richiesto.");
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

}

