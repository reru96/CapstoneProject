using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemySpawner : MonoBehaviour
{
    public GameObject enemyPrefab;
    public int spawnCount = 5;
    public float spawnRadius = 10f;
    public float spawnDelay = 2f;
    public bool spawnOnStart = true;

    public float sampleRadius = 2f; 
    public LayerMask groundMask;


    private void Start()
    {
        if (spawnOnStart)
            StartCoroutine(SpawnEnemiesRoutine());
    }

    private IEnumerator SpawnEnemiesRoutine()
    {
        for (int i = 0; i < spawnCount; i++)
        {
            SpawnEnemy();
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
