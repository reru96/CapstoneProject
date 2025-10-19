using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private Transform spawn;
    void Start()
    {
        var spawnManager = ServiceLocator.Get<PlayerSpawnManager>();
        spawnManager.SpawnPlayerFromClassSelection();
    }

}
