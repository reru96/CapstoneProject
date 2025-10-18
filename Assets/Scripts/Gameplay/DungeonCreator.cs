using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay
{
    public class DungeonCreator : MonoBehaviour
    {
        public int dungeonWidth = 50;
        public int dungeonLength = 50;
        public int roomWidthMin = 5;
        public int roomLengthMin = 5;
        public int maxIterations = 10;
        public int corridorWidth = 2;
        public int maxWallAysnc = 50;

        [Range(0.0f, 0.3f)] public float roomBottomCornerModifier = 0.1f;
        [Range(0.7f, 1.0f)] public float roomTopCornerMidifier = 0.9f;
        [Range(0, 2)] public int roomOffset = 1;

   
        public Material floorMaterial;
        [SerializeField] private List<GameObject> wallHorizontalPrefabs;
        [SerializeField] private List<GameObject> wallVerticalPrefabs;


        public NavMeshSurface navMeshSurface;
        public bool buildNavMeshOnComplete = true;

  
        public event Action OnDungeonReady;

   
        private List<Vector3Int> possibleDoorVerticalPosition;
        private List<Vector3Int> possibleDoorHorizontalPosition;
        private List<Vector3Int> possibleWallHorizontalPosition;
        private List<Vector3Int> possibleWallVerticalPosition;

        private void Start()
        {
            StartCoroutine(GenerateDungeonAsync());
        }

        private IEnumerator GenerateDungeonAsync()
        {
            DestroyAllChildren();

         
            DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonLength);
            var listOfRooms = generator.CalculateDungeon(
                maxIterations,
                roomWidthMin,
                roomLengthMin,
                roomBottomCornerModifier,
                roomTopCornerMidifier,
                roomOffset,
                corridorWidth
            );

            GameObject wallParent = new GameObject("Walls");
            wallParent.transform.SetParent(transform);

            possibleDoorVerticalPosition = new List<Vector3Int>();
            possibleDoorHorizontalPosition = new List<Vector3Int>();
            possibleWallHorizontalPosition = new List<Vector3Int>();
            possibleWallVerticalPosition = new List<Vector3Int>();

          
            for (int i = 0; i < listOfRooms.Count; i++)
            {
                CreateFloorMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);

                if (i % 2 == 0) 
                    yield return null;
            }


            yield return StartCoroutine(CreateWallsAsync(wallParent, maxWallAysnc));

            if (buildNavMeshOnComplete)
            {
                if (navMeshSurface == null)
                {
                    navMeshSurface = GetComponent<NavMeshSurface>();
                    if (navMeshSurface == null)
                        navMeshSurface = gameObject.AddComponent<NavMeshSurface>();
                }

                navMeshSurface.layerMask = LayerMask.GetMask("Floor");
                yield return new WaitForEndOfFrame();

                navMeshSurface.BuildNavMesh();
                Debug.Log("[DungeonCreator] NavMesh generata.");
            }

            Debug.Log("[DungeonCreator] Generazione dungeon completata!");
            OnDungeonReady?.Invoke();
        }

        private void CreateFloorMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner)
        {
            Vector3 bottomLeftV = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
            Vector3 bottomRightV = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
            Vector3 topLeftV = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
            Vector3 topRightV = new Vector3(topRightCorner.x, 0, topRightCorner.y);

            Vector3[] vertices = new Vector3[] { topLeftV, topRightV, bottomLeftV, bottomRightV };
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

            GameObject floor = new GameObject($"Floor_{bottomLeftCorner}", typeof(MeshFilter), typeof(MeshRenderer));
            floor.transform.SetParent(transform);
            floor.transform.localPosition = Vector3.zero;
            floor.GetComponent<MeshFilter>().mesh = mesh;
            floor.GetComponent<MeshRenderer>().material = floorMaterial;

            for (int row = (int)bottomLeftV.x; row < (int)bottomRightV.x; row++)
            {
                AddWallPositionToList(new Vector3(row, 0, bottomLeftV.z), possibleWallHorizontalPosition, possibleDoorHorizontalPosition);
            }
            for (int row = (int)topLeftV.x; row < (int)topRightCorner.x; row++)
            {
                AddWallPositionToList(new Vector3(row, 0, topRightV.z), possibleWallHorizontalPosition, possibleDoorHorizontalPosition);
            }
            for (int col = (int)bottomLeftV.z; col < (int)topLeftV.z; col++)
            {
                AddWallPositionToList(new Vector3(bottomLeftV.x, 0, col), possibleWallVerticalPosition, possibleDoorVerticalPosition);
            }
            for (int col = (int)bottomRightV.z; col < (int)topRightV.z; col++)
            {
                AddWallPositionToList(new Vector3(bottomRightV.x, 0, col), possibleWallVerticalPosition, possibleDoorVerticalPosition);
            }
        }

        private IEnumerator CreateWallsAsync(GameObject wallParent, int maxWalls)
        {
            int count = 0;

            foreach (var wallPos in possibleWallHorizontalPosition)
            {
                CreateRandomWall(wallParent, wallPos, wallHorizontalPrefabs);
                if (++count % maxWalls == 0) yield return null; 
            }

            foreach (var wallPos in possibleWallVerticalPosition)
            {
                CreateRandomWall(wallParent, wallPos, wallVerticalPrefabs);
                if (++count % maxWalls == 0) yield return null;
            }

            Debug.Log($"[DungeonCreator] Creati {count} muri totali.");
        }

        private void CreateRandomWall(GameObject parent, Vector3Int position, List<GameObject> prefabList)
        {
            if (prefabList == null || prefabList.Count == 0) return;
            var prefab = prefabList[UnityEngine.Random.Range(0, prefabList.Count)];
            Instantiate(prefab, position, Quaternion.identity, parent.transform);
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
                foreach (Transform child in transform)
                {
                    DestroyImmediate(child.gameObject);
                }
            }
        }
    }
}

