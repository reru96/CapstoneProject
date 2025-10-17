using System;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameState CurrentState { get; private set; } = GameState.None;

    public static event Action<GameState> OnGameStateChanged;
    public static event Action OnGameStarted;
    public static event Action OnGamePaused;
    public static event Action OnGameResumed;
    public static event Action OnGameOver;

    private void Awake()
    {
        CoreSystem.Instance.Container.Register<GameManager>(this);
        CoreSystem.Instance.Resolver.Resolve(this);
        OnInjected(CoreSystem.Instance.Resolver);
    }

    protected void OnInjected(ObjectResolver resolver)
    {
        ChangeState(GameState.Loading);
        Invoke(nameof(StartGame), 0.5f);
    }

    private void StartGame()
    {
        ChangeState(GameState.Playing);
        OnGameStarted?.Invoke();
    }

    public void ChangeState(GameState newState)
    {
        if (newState == CurrentState) return;

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