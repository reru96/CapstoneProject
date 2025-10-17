using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AI;

public class DungeonCreator : MonoBehaviour
{
    public int dungeonWidth = 100;
    public int dungeonLength = 100;
    public int roomWidthMin = 6;
    public int roomLengthMin = 6;
    public int maxIterations = 20;
    public int corridorWidth = 2;

    [Range(0f, 0.3f)] public float roomBottomCornerModifier = 0.2f;
    [Range(0.7f, 1.0f)] public float roomTopCornerModifier = 0.9f;
    [Range(0, 2f)] public int roomOffset = 1;

    public Material material;
    public List<GameObject> wallVertical;
    public List<GameObject> wallHorizontal;

    public NavMeshSurface navMeshSurface;

    public int roomsPerFrame = 3;

    List<Vector3Int> possibleDoorVerticalPosition;
    List<Vector3Int> possibleDoorHorizontalPosition;
    List<Vector3Int> possibleWallHorizontalPosition;
    List<Vector3Int> possibleWallVerticalPosition;

    public static event System.Action OnDungeonReady;

    void Start()
    {
        _ = CreateDungeonAsync(); 
    }

    public async Task CreateDungeonAsync()
    {
        DestroyAllChildren();

        DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonLength);
        var listOfRooms = generator.CalculateDungeon(
            maxIterations,
            roomWidthMin,
            roomLengthMin,
            roomBottomCornerModifier,
            roomTopCornerModifier,
            roomOffset,
            corridorWidth
        );

        GameObject wallParent = new GameObject("WallParent");
        wallParent.transform.parent = transform;

        possibleDoorVerticalPosition = new List<Vector3Int>();
        possibleDoorHorizontalPosition = new List<Vector3Int>();
        possibleWallHorizontalPosition = new List<Vector3Int>();
        possibleWallVerticalPosition = new List<Vector3Int>();


        for (int i = 0; i < listOfRooms.Count; i++)
        {
            CreateMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);

            if (i % roomsPerFrame == 0)
                await Task.Yield();
        }

        await CreateWallsAsync(wallParent);

        if (navMeshSurface == null)
        {
            navMeshSurface = gameObject.GetComponent<NavMeshSurface>();
            if (navMeshSurface == null)
                navMeshSurface = gameObject.AddComponent<NavMeshSurface>();

            navMeshSurface.layerMask = LayerMask.GetMask("Floor");
        }

        navMeshSurface.BuildNavMesh();

        OnDungeonReady?.Invoke();
    }

    private async Task CreateWallsAsync(GameObject wallParent)
    {
        int counter = 0;

        foreach (var wallPosition in possibleWallHorizontalPosition)
        {
            var prefab = GetRandomWallPrefab(wallHorizontal);
            CreateWall(wallParent, wallPosition, prefab);

            if (++counter % 20 == 0)
                await Task.Yield();
        }

        foreach (var wallPosition in possibleWallVerticalPosition)
        {
            var prefab = GetRandomWallPrefab(wallVertical);
            CreateWall(wallParent, wallPosition, prefab);

            if (++counter % 20 == 0)
                await Task.Yield();
        }
    }

    private GameObject GetRandomWallPrefab(List<GameObject> prefabList)
    {
        if (prefabList == null || prefabList.Count == 0)
        {
            Debug.LogWarning("Lista di prefab vuota!");
            return null;
        }
        int index = Random.Range(0, prefabList.Count);
        return prefabList[index];
    }

    private void CreateWall(GameObject wallParent, Vector3Int wallPosition, GameObject wallPrefab)
    {
        if (wallPrefab == null) return;
        Instantiate(wallPrefab, wallPosition, Quaternion.identity, wallParent.transform);
    }

    private void CreateMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner)
    {
        Vector3 bottomLeftV = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
        Vector3 bottomRightV = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
        Vector3 topLeftV = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
        Vector3 topRightV = new Vector3(topRightCorner.x, 0, topRightCorner.y);

        Vector3[] vertices = new Vector3[]
        {
            topLeftV,
            topRightV,
            bottomLeftV,
            bottomRightV
        };

        Vector2[] uvs = new Vector2[vertices.Length];
        for (int i = 0; i < uvs.Length; i++)
            uvs[i] = new Vector2(vertices[i].x, vertices[i].z);

        int[] triangles = new int[] { 0, 1, 2, 2, 1, 3 };

        Mesh mesh = new Mesh
        {
            vertices = vertices,
            uv = uvs,
            triangles = triangles
        };

        GameObject dungeonFloor = new GameObject("Mesh" + bottomLeftCorner, typeof(MeshFilter), typeof(MeshRenderer));
        dungeonFloor.transform.position = Vector3.zero;
        dungeonFloor.transform.localScale = Vector3.one;
        dungeonFloor.GetComponent<MeshFilter>().mesh = mesh;
        dungeonFloor.GetComponent<MeshRenderer>().material = material;
        dungeonFloor.transform.parent = transform;

        for (int row = (int)bottomLeftV.x; row < (int)bottomRightV.x; row++)
        {
            var wallPosition = new Vector3(row, 0, bottomLeftV.z);
            AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizontalPosition);
        }
        for (int row = (int)topLeftV.x; row < (int)topRightCorner.x; row++)
        {
            var wallPosition = new Vector3(row, 0, topRightV.z);
            AddWallPositionToList(wallPosition, possibleWallHorizontalPosition, possibleDoorHorizontalPosition);
        }
        for (int col = (int)bottomLeftV.z; col < (int)topLeftV.z; col++)
        {
            var wallPosition = new Vector3(bottomLeftV.x, 0, col);
            AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
        }
        for (int col = (int)bottomRightV.z; col < (int)topRightV.z; col++)
        {
            var wallPosition = new Vector3(bottomRightV.x, 0, col);
            AddWallPositionToList(wallPosition, possibleWallVerticalPosition, possibleDoorVerticalPosition);
        }
    }

    private void AddWallPositionToList(Vector3 wallPosition, List<Vector3Int> wallList, List<Vector3Int> doorList)
    {
        Vector3Int point = Vector3Int.CeilToInt(wallPosition);
        if (wallList.Contains(point))
        {
            doorList.Add(point);
            wallList.Remove(point);
        }
        else
        {
            wallList.Add(point);
        }
    }

    private void DestroyAllChildren()
    {
        while (transform.childCount > 0)
        {
            foreach (Transform item in transform)
            {
                DestroyImmediate(item.gameObject);
            }
        }
    }
}
