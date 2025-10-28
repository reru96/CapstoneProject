using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int spawnCount = 5;
    public float spawnRadius = 10f;
    public float spawnDelay = 3f;
    public Transform spawnPoint;

    public float activationDistance = 15f; 
    public float checkInterval = 1f;    

    public float sampleRadius = 2f;
    public LayerMask groundMask;

    private GameObject player;
    private Coroutine spawnRoutine;
    private bool isActive = false;

    private void OnEnable()
    {
        GameEvent.OnPlayerSpawned += HandlePlayerSpawned;
    }

    private void OnDisable()
    {
        GameEvent.OnPlayerSpawned -= HandlePlayerSpawned;
        StopSpawning();
    }

    private void HandlePlayerSpawned()
    {
        player = ServiceLocator.Get<PlayerSpawnManager>().Player;
        StartCoroutine(CheckPlayerDistance());
    }

    private IEnumerator CheckPlayerDistance()
    {
        while (true)
        {
            if (player == null)
                yield break;

            float distance = Vector3.Distance(transform.position, player.transform.position);

            if (distance < activationDistance)
            {
                if (!isActive)
                {
                    isActive = true;
                    spawnRoutine = StartCoroutine(SpawnEnemiesRoutine());
                }
            }
            else
            {
                if (isActive)
                {
                    isActive = false;
                    StopSpawning();
                }
            }

            yield return new WaitForSeconds(checkInterval);
        }
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            if (enemyPrefab == null)
            {
                Debug.LogWarning($"[{name}] Enemy prefab non assegnato!");
                yield break;
            }

            if (player == null || Vector3.Distance(transform.position, player.transform.position) > activationDistance)
            {
                StopSpawning();
                yield break;
            }

            SpawnEnemy();
            yield return new WaitForSeconds(spawnDelay);
        }

        isActive = false; 
    }

    private void SpawnEnemy()
    {
        Vector3 randomPos = transform.position + Random.insideUnitSphere * spawnRadius;
        randomPos.y = transform.position.y;

        Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);
    }

    private void StopSpawning()
    {
        if (spawnRoutine != null)
        {
            StopCoroutine(spawnRoutine);
            spawnRoutine = null;
        }
    }
}

