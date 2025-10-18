using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public int maxWallsPerFrame = 50;

        [Range(0f, 0.3f)] public float roomBottomCornerModifier = 0.1f;
        [Range(0.7f, 1.0f)] public float roomTopCornerModifier = 0.9f;
        [Range(0, 2)] public int roomOffset = 1;

 
        public Material floorMaterial;
        public List<GameObject> wallHorizontalPrefabs;
        public List<GameObject> wallVerticalPrefabs;

      
        public GameObject startRoomPrefab;
        public GameObject bossRoomPrefab;
        public GameObject restRoomPrefab;

     
        public NavMeshSurface navMeshSurface;
        public bool buildNavMeshOnComplete = true;

        public event Action OnDungeonReady;


        private List<Vector3Int> possibleWallHorizontalPosition;
        private List<Vector3Int> possibleWallVerticalPosition;
        private List<RoomNode> allRooms;

        private void Start()
        {
            StartCoroutine(GenerateDungeonAsync());
        }

        private IEnumerator GenerateDungeonAsync()
        {
            DestroyAllChildren();

            Debug.Log("[DungeonCreator] Inizio generazione dungeon...");

            DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonLength);
            var nodeList = generator.CalculateDungeon(
                maxIterations,
                roomWidthMin,
                roomLengthMin,
                roomBottomCornerModifier,
                roomTopCornerModifier,
                roomOffset,
                corridorWidth
            );

        
            allRooms = nodeList.OfType<RoomNode>().ToList();

   
            AssignSpecialRooms();


            possibleWallHorizontalPosition = new List<Vector3Int>();
            possibleWallVerticalPosition = new List<Vector3Int>();


            for (int i = 0; i < allRooms.Count; i++)
            {
                CreateFloorMesh(allRooms[i].BottomLeftAreaCorner, allRooms[i].TopRightAreaCorner);
                if (i % 2 == 0)
                    yield return null;
            }

 
            yield return StartCoroutine(CreateWallsAsync(maxWallsPerFrame));


            yield return StartCoroutine(SpawnSpecialRoomsAsync());

          
            if (buildNavMeshOnComplete)
            {
                if (navMeshSurface == null)
                    navMeshSurface = GetComponent<NavMeshSurface>() ?? gameObject.AddComponent<NavMeshSurface>();

                navMeshSurface.layerMask = LayerMask.GetMask("Floor");
                yield return new WaitForEndOfFrame();
                navMeshSurface.BuildNavMesh();
            }

            var spawnManager = ServiceLocator.Get<PlayerSpawnManager>();
            if (spawnManager.Player != null)
            {
                var agent = spawnManager.Player.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent != null)
                    agent.enabled = true;

                var startRoom = GameObject.FindWithTag("SpawnPoint");
                if (startRoom != null)
                    spawnManager.Player.transform.position = startRoom.transform.position + Vector3.up * 1.5f;
            }

            Debug.Log("[DungeonCreator] Generazione completata!");
            OnDungeonReady?.Invoke();
        }

        private void CreateFloorMesh(Vector2 bottomLeftCorner, Vector2 topRightCorner)
        {
            Vector3 bottomLeftV = new Vector3(bottomLeftCorner.x, 0, bottomLeftCorner.y);
            Vector3 bottomRightV = new Vector3(topRightCorner.x, 0, bottomLeftCorner.y);
            Vector3 topLeftV = new Vector3(bottomLeftCorner.x, 0, topRightCorner.y);
            Vector3 topRightV = new Vector3(topRightCorner.x, 0, topRightCorner.y);

            Vector3[] vertices = { topLeftV, topRightV, bottomLeftV, bottomRightV };
            Vector2[] uvs = vertices.Select(v => new Vector2(v.x, v.z)).ToArray();
            int[] triangles = { 0, 1, 2, 2, 1, 3 };

            Mesh mesh = new Mesh { vertices = vertices, uv = uvs, triangles = triangles };

            GameObject floor = new GameObject($"Floor_{bottomLeftCorner}", typeof(MeshFilter), typeof(MeshRenderer));
            floor.transform.SetParent(transform);
            floor.GetComponent<MeshFilter>().mesh = mesh;
            floor.GetComponent<MeshRenderer>().material = floorMaterial;

            for (int row = (int)bottomLeftV.x; row < (int)bottomRightV.x; row++)
                AddWallPosition(new Vector3(row, 0, bottomLeftV.z), possibleWallHorizontalPosition);

            for (int row = (int)topLeftV.x; row < (int)topRightCorner.x; row++)
                AddWallPosition(new Vector3(row, 0, topRightV.z), possibleWallHorizontalPosition);

            for (int col = (int)bottomLeftV.z; col < (int)topLeftV.z; col++)
                AddWallPosition(new Vector3(bottomLeftV.x, 0, col), possibleWallVerticalPosition);

            for (int col = (int)bottomRightV.z; col < (int)topRightV.z; col++)
                AddWallPosition(new Vector3(bottomRightV.x, 0, col), possibleWallVerticalPosition);
        }

        private void AddWallPosition(Vector3 wallPosition, List<Vector3Int> list)
        {
            Vector3Int point = Vector3Int.CeilToInt(wallPosition);
            if (!list.Contains(point))
                list.Add(point);
        }

        private IEnumerator CreateWallsAsync(int maxWalls)
        {
            GameObject wallParent = new GameObject("Walls");
            wallParent.transform.SetParent(transform);
            int count = 0;

            foreach (var pos in possibleWallHorizontalPosition)
            {
                CreateRandomWall(wallParent, pos, wallHorizontalPrefabs);
                if (++count % maxWalls == 0) yield return null;
            }

            foreach (var pos in possibleWallVerticalPosition)
            {
                CreateRandomWall(wallParent, pos, wallVerticalPrefabs);
                if (++count % maxWalls == 0) yield return null;
            }

            Debug.Log($"[DungeonCreator] Muri creati: {count}");
        }

        private void CreateRandomWall(GameObject parent, Vector3Int pos, List<GameObject> prefabList)
        {
            if (prefabList == null || prefabList.Count == 0) return;
            var prefab = prefabList[UnityEngine.Random.Range(0, prefabList.Count)];
            Instantiate(prefab, pos, Quaternion.identity, parent.transform);
        }

        private void AssignSpecialRooms()
        {
            if (allRooms.Count == 0) return;

            RoomNode startRoom = allRooms.OrderBy(r => r.BottomLeftAreaCorner.x + r.BottomLeftAreaCorner.y).First();
            RoomNode bossRoom = allRooms.OrderByDescending(r => r.TopRightAreaCorner.x + r.TopRightAreaCorner.y).First();

            startRoom.isStart = true;
            bossRoom.isBoss = true;

            if (allRooms.Count > 3)
            {
                RoomNode restRoom = allRooms[UnityEngine.Random.Range(1, allRooms.Count - 1)];
                restRoom.isRest = true;
            }

            Debug.Log($"[DungeonCreator] Start: {startRoom.BottomLeftAreaCorner}, Boss: {bossRoom.TopRightAreaCorner}");
        }

        private IEnumerator SpawnSpecialRoomsAsync()
        {
            SpecialRoomManager spawner = new SpecialRoomManager
            {
                startPrefab = startRoomPrefab,
                bossPrefab = bossRoomPrefab,
                restPrefab = restRoomPrefab,
                maxSpawnPerFrame = 1
            };

            yield return spawner.SpawnSpecialPrefabsAsync(allRooms);
        }

        private void DestroyAllChildren()
        {
            for (int i = transform.childCount - 1; i >= 0; i--)
                DestroyImmediate(transform.GetChild(i).gameObject);
        }
    }
}



