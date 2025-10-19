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
        [Range(0.0f, 0.3f)] public float roomBottomCornerModifier;
        [Range(0.7f, 1.0f)] public float roomTopCornerMidifier;
        [Range(0, 2)] public int roomOffset;

        [Header("References")]
        public NavMeshSurface navMeshSurface;
        public RoomPoolManager poolManager;
        public DungeonContentSpawner contentSpawner;

        [Header("Runtime Data")]
        public List<RoomNode> listOfRooms = new List<RoomNode>();
        public Transform spawnPoint;

        private void Start()
        {
            StartCoroutine(CreateDungeon());
        }

        private IEnumerator CreateDungeon()
        {
            if (poolManager == null)
            {
                Debug.LogError("[DungeonCreator] RoomPoolManager non assegnato!");
                yield break;
            }

            poolManager.ResetAll();

            DungeonGenerator generator = new DungeonGenerator(dungeonWidth, dungeonLength);
            var generated = generator.CalculateDungeon(
                maxIterations, roomWidthMin, roomLengthMin,
                roomBottomCornerModifier, roomTopCornerMidifier,
                roomOffset, corridorWidth
            );

            listOfRooms.Clear();

            for (int i = 0; i < generated.Count; i++)
            {
                if (generated[i] is RoomNode roomNode)
                {
                    listOfRooms.Add(roomNode);

                    string roomType = GetRoomTypeByPosition(i, generated.Count);
                    GameObject roomObj = poolManager.GetFromPool(roomType);
                    roomObj.transform.position = GetRoomCenter(roomNode);
                    roomObj.transform.SetParent(transform);

                    contentSpawner.SpawnContentInRoom(roomObj, roomType);

                    yield return null;
                }
            }

            RoomConnector connector = GetComponent<RoomConnector>();
            if (connector != null)
            {
                connector.ClearCorridors();
                connector.ConnectRooms(generated, transform);
            }


            if (navMeshSurface != null) navMeshSurface.BuildNavMesh();

            if (listOfRooms.Count > 0)
            {
                var firstRoom = listOfRooms[0];
                Vector3 center = GetRoomCenter(firstRoom);
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



            Debug.Log("[DungeonCreator] Dungeon creato con " + listOfRooms.Count + " stanze.");
        }

        private string GetRoomTypeByPosition(int index, int total)
        {
            if (index == 0) return "Start";
            if (index == total - 1) return "Boss";
            if (index >= total * 0.6f) return "Elite";
            return "Common";
        }

        private Vector3 GetRoomCenter(RoomNode node)
        {
            return new Vector3(
                (node.BottomLeftAreaCorner.x + node.TopRightAreaCorner.x) / 2f,
                0,
                (node.BottomLeftAreaCorner.y + node.TopRightAreaCorner.y) / 2f
            );
        }
    }

}




