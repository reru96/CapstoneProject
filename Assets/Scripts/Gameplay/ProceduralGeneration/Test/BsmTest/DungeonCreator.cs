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
        public int dungeonWidth, dungeonLength;
        public int roomWidthMin, roomLengthMin;
        public int maxIterations;
        public int corridorWidth;
        [Range(0.0f, 0.3f)] public float roomBottomCornerModifier;
        [Range(0.7f, 1.0f)] public float roomTopCornerMidifier;
        public int roomOffset;

        [Header("References")]
        public NavMeshSurface navMeshSurface;
        public RoomPoolManager poolManager;
        public DungeonContentSpawner contentSpawner;
        public RoomConnector roomConnector;

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

            RoomNode bossRoomNode = null;
            int bossIndex = generated.Count - 1; 
            if (generated[bossIndex] is RoomNode lastNode)
            {
                bossRoomNode = lastNode;

                int maxBossWidth = Mathf.Min(bossRoomNode.Width, 12);
                int maxBossLength = Mathf.Min(bossRoomNode.Length, 12);
                bossRoomNode.TopRightAreaCorner = bossRoomNode.BottomLeftAreaCorner + new Vector2Int(maxBossWidth, maxBossLength);

                generated.RemoveAt(bossIndex);
            }

   
            for (int i = 0; i < generated.Count; i++)
            {
                RoomNode roomNode = generated[i] as RoomNode;
                if (roomNode == null) continue;

                listOfRooms.Add(roomNode);

                RoomType roomType = GetRoomTypeByPosition(i, generated.Count + 1);
                GameObject roomObj = poolManager.GetFromPool(roomType);
                if (roomObj == null) continue;

                roomObj.transform.position = GetRoomCenter(roomNode);
                roomObj.transform.SetParent(transform);
                contentSpawner?.SpawnContentInRoom(roomObj, roomType);

                yield return null;
            }

   
            if (bossRoomNode != null)
            {
                listOfRooms.Add(bossRoomNode);

                GameObject bossRoomObj = poolManager.GetFromPool(RoomType.Boss);
                if (bossRoomObj != null)
                {
                    bossRoomObj.transform.position = GetRoomCenter(bossRoomNode);
                    bossRoomObj.transform.SetParent(transform);
                    contentSpawner?.SpawnContentInRoom(bossRoomObj, RoomType.Boss);
                }

   
                roomConnector?.ConnectBossRoom(bossRoomNode, listOfRooms[listOfRooms.Count - 2], transform);
            }

            if (roomConnector != null)
            {
                roomConnector.ClearCorridors();
                roomConnector.ConnectRooms(generated, transform);
            }

            if (navMeshSurface != null)
                navMeshSurface.BuildNavMesh();

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


        private RoomType GetRoomTypeByPosition(int index, int total)
        {
            if (index == 0) return RoomType.Start;
            if (index == total / 2) return RoomType.Rest;
            if (index == total - 1) return RoomType.Boss;
            if (index >= total * 0.6f) return RoomType.Elite;
            return RoomType.Common;
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




