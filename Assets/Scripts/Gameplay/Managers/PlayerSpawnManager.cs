using Core;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace Gameplay
{
    public class PlayerSpawnManager : Injectable<PlayerSpawnManager>
    {

        [SerializeField] private Transform currentRespawnPoint;
        [SerializeField] private float spawnHeightOffset = 0.5f;
        [SerializeField] private float navMeshCheckRadius = 2f;

        private GameObject player;

        public GameObject Player => player;
        public Transform CurrentRespawnPoint => currentRespawnPoint;

        protected override void Awake()
        {
            base.Awake();
            SpawnPlayerFromClassSelection();
        }

        public void SetRespawnPoint(Transform point)
        {
            currentRespawnPoint = point;
        }

        public void HandleSceneReady(DungeonCreator dungeon = null)
        {
            if (dungeon != null)
            {
                dungeon.OnDungeonReady += SpawnPlayerFromClassSelection;
            }
            else
            {
                SpawnPlayerFromClassSelection();
            }
        }

      
        private void SpawnPlayerFromClassSelection()
        {
            if (!ServiceLocator.TryGet<ClassSelectionManager>(out var classMgr))
            {
                Debug.LogWarning("[PlayerSpawnManager] ClassSelectionManager non trovato!");
                return;
            }

            if (classMgr.SelectedClass != null)
            {
                SpawnPlayer(classMgr.SelectedClass);
            }
            else
            {
                classMgr.OnClassChanged += SpawnPlayer;
            }
        }


        public void SpawnPlayer(SOPlayerClass playerClass)
        {
            if (playerClass == null || playerClass.prefab == null)
            {
                Debug.LogWarning("[PlayerSpawnManager] Prefab della classe selezionata mancante!");
                return;
            }

            if (player != null) Destroy(player);

            Vector3 spawnPos = currentRespawnPoint != null ? currentRespawnPoint.position : Vector3.zero;
            spawnPos += Vector3.up * spawnHeightOffset;

            if (NavMesh.SamplePosition(spawnPos, out var hit, navMeshCheckRadius, NavMesh.AllAreas))
                spawnPos = hit.position;

            player = Instantiate(playerClass.prefab, spawnPos, Quaternion.identity);
            player.name = "Player";

            var agent = player.GetComponent<NavMeshAgent>();
            if (agent != null && !agent.isOnNavMesh) agent.enabled = false;

            Debug.Log($"[PlayerSpawnManager] Player spawnato in {spawnPos}");
        }
    }
}

