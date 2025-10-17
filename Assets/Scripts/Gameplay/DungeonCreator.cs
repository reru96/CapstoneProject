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

        [Range(0f, 0.3f)] public float roomBottomCornerModifier = 0.1f;
        [Range(0.7f, 1.0f)] public float roomTopCornerModifier = 0.9f;
        [Range(0, 2f)] public int roomOffset = 1;

        public Material floorMaterial;

        public NavMeshSurface navMeshSurface;

        public event Action OnDungeonReady;

        private List<Vector3Int> possibleWallHorizontal;
        private List<Vector3Int> possibleWallVertical;

        private void Start()
        {
            StartCoroutine(GenerateDungeonAsync());
        }

        private IEnumerator GenerateDungeonAsync()
        {
            DestroyAllChildren();

            DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonLength);
            var listOfRooms = generator.CalculateDungeon(maxIterations,
                                                         roomWidthMin,
                                                         roomLengthMin,
                                                         roomBottomCornerModifier,
                                                         roomTopCornerModifier,
                                                         roomOffset,
                                                         corridorWidth);

            possibleWallHorizontal = new List<Vector3Int>();
            possibleWallVertical = new List<Vector3Int>();

            for (int i = 0; i < listOfRooms.Count; i++)
            {
                CreateFloorMesh(listOfRooms[i].BottomLeftAreaCorner, listOfRooms[i].TopRightAreaCorner);

                if (i % 2 == 0) yield return null;
            }

     
            if (navMeshSurface == null)
            {
                navMeshSurface = GetComponent<NavMeshSurface>();
                if (navMeshSurface == null)
                {
                    navMeshSurface = gameObject.AddComponent<NavMeshSurface>();
                }
            }
            navMeshSurface.layerMask = LayerMask.GetMask("Floor");

            yield return new WaitForEndOfFrame();
            navMeshSurface.BuildNavMesh();

            Debug.Log("[AsyncDungeonGenerator] Dungeon generato, NavMesh pronta!");
            OnDungeonReady?.Invoke();
        }

        private void CreateFloorMesh(Vector2 bottomLeft, Vector2 topRight)
        {
            Vector3 bl = new Vector3(bottomLeft.x, 0, bottomLeft.y);
            Vector3 br = new Vector3(topRight.x, 0, bottomLeft.y);
            Vector3 tl = new Vector3(bottomLeft.x, 0, topRight.y);
            Vector3 tr = new Vector3(topRight.x, 0, topRight.y);

            Vector3[] vertices = { tl, tr, bl, br };
            Vector2[] uvs = new Vector2[4];
            for (int i = 0; i < 4; i++) uvs[i] = new Vector2(vertices[i].x, vertices[i].z);
            int[] triangles = { 0, 1, 2, 2, 1, 3 };

            Mesh mesh = new Mesh { vertices = vertices, uv = uvs, triangles = triangles };

            GameObject floor = new GameObject($"Floor_{bottomLeft}");
            floor.transform.parent = transform;
            floor.transform.localPosition = Vector3.zero;
            floor.transform.localScale = Vector3.one;

            MeshFilter mf = floor.AddComponent<MeshFilter>();
            mf.mesh = mesh;

            MeshRenderer mr = floor.AddComponent<MeshRenderer>();
            mr.material = floorMaterial;
        }

        private void DestroyAllChildren()
        {
            while (transform.childCount > 0)
            {
                for (int i = transform.childCount - 1; i >= 0; i--)
                {
                    DestroyImmediate(transform.GetChild(i).gameObject);
                }
            }
        }
    }
}
