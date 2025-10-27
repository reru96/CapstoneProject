using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;

public class BossSpawner : MonoBehaviour
{
    public SOEnemy bossPrefab;      
    public Transform spawnPoint;       
    public float activationRange = 15f;

    private PlayerSpawnManager spawnManager;
    private GameObject currentBoss;

    void Start()
    {
        spawnManager = ServiceLocator.TryGet<PlayerSpawnManager>();
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, spawnManager.Player.transform.position);

        if (distance < activationRange)
        {
            if (currentBoss == null)
            {
                currentBoss = Instantiate(bossPrefab.enemyPrefab, spawnPoint.position, spawnPoint.rotation);
            }
        }
        else
        {
           
            if (currentBoss != null)
            {
                Destroy(currentBoss);
                currentBoss = null;
            }
        }
    }
}
