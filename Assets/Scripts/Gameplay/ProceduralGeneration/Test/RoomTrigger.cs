using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomTrigger : MonoBehaviour
{
    [HideInInspector] public DungeonManager dungeonManager;
    public Vector3 spawnOffset = new Vector3(15, 0, 0);
    [HideInInspector] public Room roomData;

    private bool triggered = false;

    private void OnTriggerEnter(Collider other)
    {
        if (triggered) return;

        if (other.CompareTag("Player"))
        {
            triggered = true;
            Vector3 spawnPosition = transform.position + spawnOffset;
            dungeonManager.SpawnNextRoom(spawnPosition);
            GetComponent<Collider>().enabled = false;
        }
    }

}
