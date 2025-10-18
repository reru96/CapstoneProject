using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpecialRoomManager : MonoBehaviour
{
    public GameObject startPrefab;
    public GameObject bossPrefab;
    public GameObject restPrefab;
    public int maxSpawnPerFrame = 1;

    public IEnumerator SpawnSpecialPrefabsAsync(List<RoomNode> rooms)
    {
        int count = 0;

        foreach (var room in rooms)
        {
            GameObject prefabToSpawn = null;

            if (room.isStart && startPrefab != null)
                prefabToSpawn = startPrefab;
            else if (room.isBoss && bossPrefab != null)
                prefabToSpawn = bossPrefab;
            else if (room.isRest && restPrefab != null)
                prefabToSpawn = restPrefab;

            if (prefabToSpawn != null)
            {
                Vector3 spawnPos = room.CenterPosition();
                GameObject instance = GameObject.Instantiate(prefabToSpawn, spawnPos, Quaternion.identity);
                instance.name = $"{prefabToSpawn.name}_{room.TreeLayerIndex}";
            }

            if (++count % maxSpawnPerFrame == 0)
                yield return null;
        }
    }
}
