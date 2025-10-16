using System.Collections.Generic;
using log4net.Util;
using Unity.AI.Navigation;
using UnityEngine;

public class CorridorController : MonoBehaviour
{
    public SORoom corridorData;
    public RoomController roomA;
    public RoomController roomB;

    public void BuildCorridor()
    {
        Vector3 start = roomA.transform.position;
        Vector3 end = roomB.transform.position;
        Vector3 dir = end - start;

        GameObject floorPrefab = corridorData.floorPrefabs[Random.Range(0, corridorData.floorPrefabs.Count)];
        int steps = Mathf.CeilToInt(dir.magnitude / corridorData.tileSize);

        for (int i = 0; i <= steps; i++)
        {
            Vector3 pos = start + dir.normalized * i * corridorData.tileSize;
            Instantiate(floorPrefab, pos, Quaternion.identity, transform);
        }
    }
}