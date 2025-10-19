using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;

public class DungeonContentSpawner : MonoBehaviour
{

    [Header("Dungeon Content")]
    public List<SODungeonContent> contentPresets = new List<SODungeonContent>();

    private Dictionary<string, List<GameObject>> contentDictionary = new Dictionary<string, List<GameObject>>();

    private void Awake()
    {
        foreach (var content in contentPresets)
        {
            if (content == null || string.IsNullOrEmpty(content.roomType))
                continue;

            if (!contentDictionary.ContainsKey(content.roomType))
                contentDictionary[content.roomType] = new List<GameObject>();

            contentDictionary[content.roomType].AddRange(content.possibleContents);
        }
    }

    public void SpawnContentInRoom(GameObject room, string roomType)
    {
        if (!contentDictionary.ContainsKey(roomType))
        {
            Debug.LogWarning($"[DungeonContentSpawner] Nessun contenuto per il tipo {roomType}");
            return;
        }

        var contents = contentDictionary[roomType];
        if (contents.Count == 0) return;

        int spawnCount = Random.Range(1, Mathf.Min(3, contents.Count + 1));
        for (int i = 0; i < spawnCount; i++)
        {
            var prefab = contents[Random.Range(0, contents.Count)];
            var spawnPos = room.transform.position + new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3));
            Instantiate(prefab, spawnPos, Quaternion.identity, room.transform);
        }
    }
}

