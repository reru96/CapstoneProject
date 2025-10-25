using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;

public class RoomConnector : MonoBehaviour
{
    [SerializeField] private float corridorWidth = 2f;
    [SerializeField] private GameObject corridorPrefab;
    [SerializeField] private NavMeshSurface navMeshSurface;

    private List<GameObject> spawnedCorridors = new List<GameObject>();

    public void ConnectRooms(List<Node> nodes, Transform parent)
    {
        if (corridorPrefab == null)
        {
            Debug.LogWarning("[RoomConnector] Nessun prefab corridoio assegnato!");
            return;
        }

        foreach (var node in nodes)
        {
            if (node is CorridorNode corridor)
            {
                Vector3 start = new Vector3(corridor.BottomLeftAreaCorner.x, 0, corridor.BottomLeftAreaCorner.y);
                Vector3 end = new Vector3(corridor.TopRightAreaCorner.x, 0, corridor.TopRightAreaCorner.y);

                Vector3 center = (start + end) / 2f;
                Vector3 size = new Vector3(
                    Mathf.Abs(end.x - start.x),
                    3f,
                    Mathf.Abs(end.z - start.z)
                );

                var go = Instantiate(corridorPrefab, center, Quaternion.identity, parent);
                go.transform.localScale = size;
                spawnedCorridors.Add(go);
            }
        }

        if (navMeshSurface != null)
        {
            navMeshSurface.BuildNavMesh();
            Debug.Log("[RoomConnector] NavMesh aggiornata dopo la generazione dei corridoi.");
        }
    }
    public void ConnectBossRoom(RoomNode bossRoom, RoomNode lastNormalRoom, Transform parent)
    {
        if (bossRoom == null || lastNormalRoom == null) return;

        Vector2Int bossCenter = (bossRoom.BottomLeftAreaCorner + bossRoom.TopRightAreaCorner) / 2;
        Vector2Int lastCenter = (lastNormalRoom.BottomLeftAreaCorner + lastNormalRoom.TopRightAreaCorner) / 2;

        Vector3 startPos = new Vector3(lastCenter.x, 0, lastCenter.y);
        Vector3 endPos = new Vector3(bossCenter.x, 0, bossCenter.y);

        Vector3 corridorCenter = (startPos + endPos) / 2f;
        Vector3 corridorSize = new Vector3(
            Mathf.Abs(startPos.x - endPos.x) + corridorWidth,
            1f,
            Mathf.Abs(startPos.z - endPos.z) + corridorWidth
        );

        GameObject corridor = GameObject.CreatePrimitive(PrimitiveType.Cube);
        corridor.name = "BossCorridor";
        corridor.transform.position = corridorCenter;
        corridor.transform.localScale = corridorSize;
        corridor.transform.SetParent(parent);

        corridor.layer = LayerMask.NameToLayer("Dungeon");
        Destroy(corridor.GetComponent<BoxCollider>()); 

        spawnedCorridors.Add(corridor);
    }

    public void ClearCorridors()
    {
        foreach (var c in spawnedCorridors)
        {
            if (c != null) Destroy(c);
        }
        spawnedCorridors.Clear();
    }
}
