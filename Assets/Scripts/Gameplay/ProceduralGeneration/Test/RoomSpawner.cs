using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSpawner : MonoBehaviour
{
    public GameObject floor;
    public float width;
    public float lenght;
    public Vector3 move;
    public Transform parent;

    private void Start()
    {
        for (int i = 0; i < lenght; i++)
        {
            for (int x = 0; x < width; x++)
            {
                move = new Vector3(x, 0, i);
                Instantiate(floor, parent.position + move, parent.rotation);
            }

        }
    }
}
