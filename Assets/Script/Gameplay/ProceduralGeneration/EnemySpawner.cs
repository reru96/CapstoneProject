using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{

    [Header("Impostazioni nemici")]
    public List<GameObject> enemyPrefabs;
    public int minEnemies = 2;
    public int maxEnemies = 5;
    public Vector3 areaSize = new Vector3(10, 1, 10);

    private List<GameObject> activeEnemies = new List<GameObject>();

    public void SpawnEnemies(RoomController room)
    {
        ClearEnemies();
        int count = Random.Range(minEnemies, maxEnemies + 1);

        for (int i = 0; i < count; i++)
        {
            Vector3 randomPos = transform.position + new Vector3(
                Random.Range(-areaSize.x / 2, areaSize.x / 2),
                0,
                Random.Range(-areaSize.z / 2, areaSize.z / 2)
            );

            GameObject prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];
            GameObject enemy = Instantiate(prefab, randomPos, Quaternion.identity);

            EnemyStateMachine e = enemy.GetComponent<EnemyStateMachine>();
            e.currentRoom = room;

            activeEnemies.Add(enemy);
        }
    }

    public bool AllEnemiesDefeated()
    {
        activeEnemies.RemoveAll(e => e == null);
        return activeEnemies.Count == 0;
    }

    public void ClearEnemies()
    {
        foreach (var e in activeEnemies)
            if (e != null) Destroy(e);
        activeEnemies.Clear();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, areaSize);
    }
}
