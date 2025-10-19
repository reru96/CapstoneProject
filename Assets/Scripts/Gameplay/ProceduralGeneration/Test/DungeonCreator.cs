using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using Unity.AI.Navigation;
using UnityEngine;

namespace Gameplay
{
    public class DungeonCreator : MonoBehaviour
    {
        [Header("Dungeon Settings")]
        public int dungeonWidth, dungeonLength;
        public int roomWidthMin, roomLengthMin;
        public int maxIterations;
        public int corridorWidth;
        public Material material;
        [Range(0.0f, 0.3f)] public float roomBottomCornerModifier;
        [Range(0.7f, 1.0f)] public float roomTopCornerMidifier;
        [Range(0, 2)] public int roomOffset;
        public GameObject wallVertical, wallHorizontal;

        [Header("References")]
        [SerializeField] private NavMeshSurface navMeshSurface;
        [SerializeField] private RoomPoolManager roomPoolManager;
        [SerializeField] private DungeonContentSpawner contentSpawner;

        [Header("Runtime Data")]
        public List<RoomNode> listOfRooms = new List<RoomNode>();
        public Transform spawnPoint;

        private bool isGenerating = false;

        private void Start()
        {
            StartCoroutine(CreateDungeonStepByStep());
        }

        private IEnumerator CreateDungeonStepByStep()
        {
            if (isGenerating) yield break;
            isGenerating = true;

            if (roomPoolManager != null)
                roomPoolManager.ResetAll();

            DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonLength);
            var generated = generator.CalculateDungeon(
                maxIterations,
                roomWidthMin,
                roomLengthMin,
                roomBottomCornerModifier,
                roomTopCornerMidifier,
                roomOffset,
                corridorWidth
            );

            listOfRooms.Clear();

            for (int i = 0; i < generated.Count; i++)
            {
                if (generated[i] is RoomNode roomNode)
                {
                    listOfRooms.Add(roomNode);

                    Transform room = roomPoolManager.SpawnRoom();
                    room.position = new Vector3(
                        (roomNode.BottomLeftAreaCorner.x + roomNode.TopRightAreaCorner.x) / 2f,
                        0f,
                        (roomNode.BottomLeftAreaCorner.y + roomNode.TopRightAreaCorner.y) / 2f
                    );

                    if (contentSpawner != null)
                        contentSpawner.SpawnContentsInRoom(room);
                }

                if (i % 2 == 1 || i == generated.Count - 1)
                {
                    if (navMeshSurface != null)
                        navMeshSurface.BuildNavMesh();
                    yield return null;
                }
            }

            if (listOfRooms.Count > 0)
            {
                var firstRoom = listOfRooms[0];
                Vector3 center = new Vector3(
                    (firstRoom.BottomLeftAreaCorner.x + firstRoom.TopRightAreaCorner.x) / 2f,
                    0f,
                    (firstRoom.BottomLeftAreaCorner.y + firstRoom.TopRightAreaCorner.y) / 2f
                );

                spawnPoint = new GameObject("PlayerSpawnPoint").transform;
                spawnPoint.position = center + Vector3.up * 0.5f;
                spawnPoint.SetParent(transform);

                var spawner = ServiceLocator.Get<PlayerSpawnManager>();
                if (spawner != null)
                {
                    spawner.SetRespawnPoint(spawnPoint);
                    spawner.SpawnPlayerFromClassSelection();
                }
            }

            isGenerating = false;
        }

        public RoomNode GetFirstRoom() =>
            listOfRooms.Count > 0 ? listOfRooms[0] : null;
    }
}




