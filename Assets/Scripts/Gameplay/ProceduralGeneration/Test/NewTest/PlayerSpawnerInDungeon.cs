using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using Gameplay;
using System.Threading;

public class PlayerSpawnerInDungeon : MonoBehaviour
{
    public Transform spawnPoint;

    public void Start()
    {
        Debug.Log("[PlayerSpawnerInDungeon] Start chiamato");
        GameEvent.OnDungeonReady += InitSpawn;
    }

    private void OnDisable()
    {
        GameEvent.OnDungeonReady -= InitSpawn;
    }

    private void InitSpawn()
    {
        StartCoroutine(NotifySpawn());
    }

    private IEnumerator NotifySpawn()
    {
        yield return new WaitForSeconds(5);
        Debug.Log("[PlayerSpawnerInDungeon] InitSpawn chiamato");
        var player = ServiceLocator.Get<PlayerSpawnManager>();
        if (player == null)
        {
            Debug.LogError("[PlayerSpawnerInDungeon] PlayerSpawnManager non trovato!");
            yield return null;
        }
        var classMgr = ServiceLocator.Get<ClassSelectionManager>();
        player.SetRespawnPoint(spawnPoint);
        player.SpawnPlayer(classMgr.SelectedClass);
    }
}
