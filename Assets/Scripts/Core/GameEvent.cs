using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvent 
{
    public static Action OnDungeonReady;

    public static Action OnPlayerSpawned;

    public static Action OnPlayerDead;
    
    public static Action OnBossDead;

    public static void PlayerSpawned()
    {
        OnPlayerSpawned?.Invoke();
    }

    public static void DungeonReady()
    {
        Debug.Log($"[GameEvent] DungeonReady invocato. Listener attivi: {OnDungeonReady?.GetInvocationList().Length ?? 0}");
        OnDungeonReady?.Invoke(); 
    }

    public static void PlayerDead()
    { 
        OnPlayerDead?.Invoke();
    }

    public static void BossDead() 
    {
        OnBossDead?.Invoke();
    }

}
