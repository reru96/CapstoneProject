using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class DoorLinker : MonoBehaviour
{
    public RoomController targetRoom;
    public Direction targetDirection;

    public float teleportOffset = 2f;
    public NavMeshLink navLink; 

    private void Awake()
    {
       
        Collider col = GetComponent<Collider>();
        if (col != null)
            col.isTrigger = true;
    }

    private void Start()
    {

        if (navLink == null)
        {
            navLink = GetComponent<NavMeshLink>();
            if (navLink == null)
                navLink = gameObject.AddComponent<NavMeshLink>();
        }

        navLink.bidirectional = true;
        navLink.width = 2f;
        navLink.costModifier = -1;
        navLink.area = 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player") || targetRoom == null)
            return;

        Vector3 offset = Vector3.zero;
        switch (targetDirection)
        {
            case Direction.North: offset = Vector3.forward * teleportOffset; break;
            case Direction.South: offset = Vector3.back * teleportOffset; break;
            case Direction.East: offset = Vector3.right * teleportOffset; break;
            case Direction.West: offset = Vector3.left * teleportOffset; break;
        }

        Transform player = other.transform;
        player.position = targetRoom.transform.position + offset;
        player.rotation = Quaternion.LookRotation(-offset.normalized, Vector3.up);
    }

}
