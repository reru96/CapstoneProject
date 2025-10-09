using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class PlayerSpawnManager : Injectable<PlayerSpawnManager>
{
    [SerializeField] private float spawnHeightOffset = 0.5f;
    [SerializeField] private float navMeshCheckRadius = 2f;
    [SerializeField] private float navMeshCheckTimeout = 5f;

    private Transform _respawnPoint;
    private GameObject _player;

    public GameObject Player => _player;
    public Transform RespawnPoint => _respawnPoint;

    public event System.Action<GameObject> OnPlayerSpawned;

    protected override void OnInjected(ObjectResolver resolver)
    {
        base.OnInjected(resolver);
        SceneManager.sceneLoaded += OnSceneLoaded;
        FindRespawnPoint();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindRespawnPoint();

        var classMgr = Resolve<ClassSelectionManager>();
        if (classMgr.SelectedClass?.prefab != null)
        {
            StartCoroutine(SpawnWhenReady(classMgr.SelectedClass.prefab));
        }
    }

    private void FindRespawnPoint()
    {
        var respawnObj = GameObject.FindGameObjectWithTag("RespawnPoint");
        if (respawnObj == null)
        {
            respawnObj = new GameObject("FallbackRespawnPoint");
            respawnObj.transform.position = Vector3.zero;
        }
        _respawnPoint = respawnObj.transform;
    }

    public IEnumerator SpawnWhenReady(GameObject prefab)
    {
        yield return WaitForValidSpawnPosition();
        SpawnPlayer(prefab);
    }

    private IEnumerator WaitForValidSpawnPosition()
    {
        if (_respawnPoint == null) yield break;
        float startTime = Time.time;
        bool positionValid = false;
        Vector3 validPosition = _respawnPoint.position;

        while (Time.time - startTime < navMeshCheckTimeout && !positionValid)
        {
            positionValid = NavMesh.SamplePosition(_respawnPoint.position, out var hit, navMeshCheckRadius, NavMesh.AllAreas);
            if (positionValid) validPosition = hit.position;
            else yield return new WaitForSeconds(0.1f);
        }

        _respawnPoint.position = validPosition;
    }

    private void SpawnPlayer(GameObject prefab)
    {
        if (_player != null) Destroy(_player);

        Vector3 spawnPos = _respawnPoint.position + Vector3.up * spawnHeightOffset;
        if (NavMesh.SamplePosition(spawnPos, out var hit, navMeshCheckRadius, NavMesh.AllAreas))
            spawnPos = hit.position;

        _player = Instantiate(prefab, spawnPos, Quaternion.identity);
        _player.name = "Player";
        OnPlayerSpawned?.Invoke(_player);
    }
}
