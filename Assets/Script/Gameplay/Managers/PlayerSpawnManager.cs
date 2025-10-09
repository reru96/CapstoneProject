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
    public event System.Action<GameObject> OnPlayerSpawned;

    protected override void OnInjected(ObjectResolver resolver)
    {
        base.OnInjected(resolver);
        SceneManager.sceneLoaded += OnSceneLoaded;
        DontDestroyOnLoad(this.gameObject); 
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
        if (classMgr == null)
        {
            Debug.LogError("[PlayerSpawnManager] ClassSelectionManager non trovato!");
            return;
        }

        if (classMgr.SelectedClass != null)
        {
            StartCoroutine(SpawnWhenReady(classMgr.SelectedClass));
        }
        else
        {
            classMgr.OnClassChanged += HandleClassSelected;
        }
    }

    private void HandleClassSelected(SOPlayerClass selectedClass)
    {
        var classMgr = Resolve<ClassSelectionManager>();
        classMgr.OnClassChanged -= HandleClassSelected; 
        StartCoroutine(SpawnWhenReady(selectedClass));
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

    public IEnumerator SpawnWhenReady(SOPlayerClass playerClass)
    {
        if (playerClass?.prefab == null)
        {
            Debug.LogError("[PlayerSpawnManager] Prefab della classe selezionata mancante!");
            yield break;
        }

        yield return new WaitForEndOfFrame();       
        yield return WaitForNavMeshReady();         
        yield return WaitForValidSpawnPosition();   

        SpawnPlayer(playerClass);
    }

    private IEnumerator WaitForNavMeshReady()
    {
        float start = Time.time;
        while (!NavMesh.SamplePosition(Vector3.zero, out _, 10f, NavMesh.AllAreas))
        {
            if (Time.time - start > navMeshCheckTimeout)
            {
                Debug.LogWarning("[PlayerSpawnManager] Timeout attesa NavMesh, procedo comunque.");
                yield break;
            }
            yield return null;
        }
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
            if (positionValid)
                validPosition = hit.position;
            else
                yield return new WaitForSeconds(0.1f);
        }

        if (!positionValid)
            Debug.LogWarning("[PlayerSpawnManager] Nessuna posizione valida sul NavMesh, uso posizione originale.");

        _respawnPoint.position = validPosition;
    }

    private void SpawnPlayer(SOPlayerClass playerClass)
    {
        if (_player != null) Destroy(_player);

        Vector3 spawnPos = _respawnPoint.position + Vector3.up * spawnHeightOffset;
        if (NavMesh.SamplePosition(spawnPos, out var hit, navMeshCheckRadius, NavMesh.AllAreas))
            spawnPos = hit.position;

        _player = Instantiate(playerClass.prefab, spawnPos, Quaternion.identity);
        _player.name = "Player";

        var agent = _player.GetComponent<NavMeshAgent>();
        if (agent != null && !agent.isOnNavMesh)
        {
            Debug.LogWarning("[PlayerSpawnManager] Player non su NavMesh, disabilito temporaneamente NavMeshAgent.");
            agent.enabled = false;
        }

        OnPlayerSpawned?.Invoke(_player);
        Debug.Log($"[PlayerSpawnManager] Player spawnato in {spawnPos}");
    }
}

