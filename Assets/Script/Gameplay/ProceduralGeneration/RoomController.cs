using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    [Header("Porte")]
    public GameObject northDoor;
    public GameObject southDoor;
    public GameObject eastDoor;
    public GameObject westDoor;

    [Header("Enemy Spawner")]
    public EnemySpawner enemySpawner;

    private Vector3Int gridPos;
    private Dictionary<Vector3Int, RoomNode> allRooms;
    private bool roomCleared = false;
    private bool playerInside = false;

    private List<GameObject> doors = new List<GameObject>();

    public void Initialize(Vector3Int pos, Dictionary<Vector3Int, RoomNode> rooms)
    {
        gridPos = pos;
        allRooms = rooms;

        doors = new List<GameObject> { northDoor, southDoor, eastDoor, westDoor };

        northDoor.SetActive(allRooms.ContainsKey(gridPos + Vector3Int.forward));
        southDoor.SetActive(allRooms.ContainsKey(gridPos + Vector3Int.back));
        eastDoor.SetActive(allRooms.ContainsKey(gridPos + Vector3Int.right));
        westDoor.SetActive(allRooms.ContainsKey(gridPos + Vector3Int.left));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !playerInside)
        {
            playerInside = true;
            OnPlayerEnter();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
            playerInside = false;
    }

    public void OnPlayerEnter()
    {
        if (roomCleared) return;

        CloseDoors();
        enemySpawner.SpawnEnemies(this);
    }

    public void OnEnemyDefeated()
    {
        if (enemySpawner.AllEnemiesDefeated())
        {
            roomCleared = true;
            OpenDoors();
        }
    }

    void CloseDoors()
    {
        foreach (var door in doors)
            if (door && door.activeSelf)
                door.GetComponent<Collider>().enabled = true;
    }

    void OpenDoors()
    {
        foreach (var door in doors)
            if (door && door.activeSelf)
                door.GetComponent<Collider>().enabled = false;
    }
}
