using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;

public class DungeonContentSpawner : MonoBehaviour
{

    [Header("Content Settings")]
    public List<SODungeonContent> possibleContents;

    public void SpawnContentsInRoom(Transform room)
    {
        if (room == null || possibleContents == null || possibleContents.Count == 0)
            return;

    
        SODungeonContent content = possibleContents[Random.Range(0, possibleContents.Count)];
        if (content == null) return;

        foreach (var entry in content.contents)
        {
            if (entry.prefab == null) continue;

            Vector3 spawnPos = room.position + entry.localPosition;
            Quaternion spawnRot = Quaternion.Euler(entry.localRotation);
            GameObject go = Instantiate(entry.prefab, spawnPos, spawnRot, room);
        }
    }
}

