using UnityEngine;
using Core;

namespace Gameplay
{
    public class RespawnManager : MonoBehaviour
    {
        private PlayerSpawnManager _playerSpawnManager;

        private void Awake()
        {
            _playerSpawnManager = ServiceLocator.Get<PlayerSpawnManager>();

            ServiceLocator.Register(this);
        }

        public void UpdateCurrentSpawnPoint(Transform checkpoint)
        {
            if (checkpoint == null)
            {
                Debug.LogWarning("[RespawnManager] Checkpoint nullo!");
                return;
            }

            _playerSpawnManager.SetRespawnPoint(checkpoint);
        }

        public void RespawnPlayerAtCurrent()
        {
            if (_playerSpawnManager.CurrentRespawnPoint == null)
            {
                Debug.LogWarning("[RespawnManager] Nessun checkpoint assegnato!");
                return;
            }

            RespawnPlayerAt(_playerSpawnManager.CurrentRespawnPoint);
        }

        public void RespawnPlayerAt(Transform checkpoint)
        {
            if (checkpoint == null)
            {
                Debug.LogWarning("[RespawnManager] Checkpoint nullo!");
                return;
            }

            var classMgr = ServiceLocator.Get<ClassSelectionManager>();
            if (classMgr.SelectedClass == null)
            {
                Debug.LogWarning("[RespawnManager] Nessuna classe selezionata, impossibile spawnare il player.");
                return;
            }

            _playerSpawnManager.SetRespawnPoint(checkpoint);
            _playerSpawnManager.SpawnPlayer(classMgr.SelectedClass);
        }
    }
}
