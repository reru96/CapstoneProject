using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    private RoomController parentRoom;
    private string direction;

    public void Initialize(RoomController room, string dir)
    {
        parentRoom = room;
        direction = dir;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            parentRoom.EnterDoor(direction, other.gameObject);
        }
    }
}
