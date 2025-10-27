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
    public float spawnDelay = 2f;
    public float maxDistance = 10f;

    public float sampleRadius = 2f;
    public LayerMask groundMask;
    private PlayerSpawnManager playerSpawnManager;

    private void Start()
    {
        playerSpawnManager = ServiceLocator.TryGet<PlayerSpawnManager>();
    }
    private void Update()
    {
        StartCoroutine(SpawnEnemiesRoutine());
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        int enemiesToSpawn = 5;
        float spawnDelay = 5f;

        for (int i = 0; i < enemiesToSpawn; i++)
        {

            float distance = Vector3.Distance(transform.position, playerSpawnManager.Player.transform.position);
            if (distance < maxDistance)
            {
                SpawnEnemy();
            }
            else
            {
                yield break;
            }

            yield return new WaitForSeconds(spawnDelay);
        }
    }

    public void SpawnEnemy()
    {
        if (enemyPrefab == null)
        {
            Debug.LogWarning("Enemy prefab non assegnato!");
            return;
        }

        Vector3 randomPos = transform.position + Random.insideUnitSphere * spawnRadius;
        randomPos.y = transform.position.y;


        if (NavMesh.SamplePosition(randomPos, out NavMeshHit hit, sampleRadius, NavMesh.AllAreas))
        {
            GameObject enemy = Instantiate(enemyPrefab, hit.position, Quaternion.identity);
        }
        else
        {
            Debug.Log("Nessuna NavMesh trovata vicino al punto di spawn.");
        }
    }
}
