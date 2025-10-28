using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public Transform spawnPoint;

    private void Start()
    {
        var playerSpawner = ServiceLocator.Get<PlayerSpawnManager>();
        playerSpawner.SetRespawnPoint(spawnPoint);
        playerSpawner.SpawnPlayerFromClassSelection();
    }
}
