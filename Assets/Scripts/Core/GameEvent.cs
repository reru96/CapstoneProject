using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameEvent 
{
    public static Action OnDungeonReady;

    public static void DungeonReady()
    {
        Debug.Log($"[GameEvent] DungeonReady invocato. Listener attivi: {OnDungeonReady?.GetInvocationList().Length ?? 0}");
        OnDungeonReady?.Invoke(); 
    }
}
