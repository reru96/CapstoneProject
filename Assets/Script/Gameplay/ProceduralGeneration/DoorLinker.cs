using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorLinker : MonoBehaviour
{
    public RoomController targetRoom;
    public Direction targetDirection;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && targetRoom != null)
        {
            Vector3 offset = Vector3.zero;
            switch (targetDirection)
            {
                case Direction.North: offset = Vector3.forward * 2f; break;
                case Direction.South: offset = Vector3.back * 2f; break;
                case Direction.East: offset = Vector3.right * 2f; break;
                case Direction.West: offset = Vector3.left * 2f; break;
            }

            other.transform.position = targetRoom.transform.position + offset;
        }
    }
}
