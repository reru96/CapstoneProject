using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public SOEnemy boss;
    public Transform spawnPoint;
    public float activationRange = 1f;

    private PlayerSpawnManager spawnManager;
    private GameObject currentBoss;
    private Coroutine checkRoutine;
    private bool bossSpawned = false;

    private void OnEnable()
    {
        GameEvent.OnPlayerSpawned += HandlePlayerSpawned;
    }

    private void OnDisable()
    {
        GameEvent.OnPlayerSpawned -= HandlePlayerSpawned;
        StopCheckRoutine();
        DestroyBoss();
    }

    private void HandlePlayerSpawned()
    {
        spawnManager = ServiceLocator.Get<PlayerSpawnManager>();
        checkRoutine = StartCoroutine(CheckPlayerDistanceRoutine());
    }

    private IEnumerator CheckPlayerDistanceRoutine()
    {
        while (true)
        {
            if (spawnManager?.Player == null)
                yield break;

            float distance = Vector3.Distance(transform.position, spawnManager.Player.transform.position);

            if (distance < activationRange)
            {
                if (!bossSpawned)
                {
                    SpawnBoss();
                }
            }
            else
            {
                if (bossSpawned)
                {
                    EndBossFight();
                }
            }

            yield return new WaitForSeconds(1f);
        }
    }

    private void SpawnBoss()
    {
        currentBoss = Instantiate(boss.enemyPrefab, spawnPoint.position, spawnPoint.rotation);
        bossSpawned = true;
        Debug.Log("[BossSpawner] Boss spawnato!");
    }

    private void EndBossFight()
    {
        DestroyBoss();
        bossSpawned = false;
        Debug.Log("[BossSpawner] Boss distrutto perché il giocatore è uscito dalla stanza.");
    }

    private void DestroyBoss()
    {
        if (currentBoss != null)
        {
            Destroy(currentBoss);
            currentBoss = null;
        }
    }

    private void StopCheckRoutine()
    {
        if (checkRoutine != null)
        {
            StopCoroutine(checkRoutine);
            checkRoutine = null;
        }
    }
}

