using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnManager : MonoBehaviour 
{
    [Header("Tentativi")]
    [SerializeField] private int maxTry = 3;
    private int leftTry;

    public int LeftTry => leftTry;
    public int MaxTry => maxTry;

    [Header("Respawn Settings")]
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private float respawnDelay = 2f;

    private GameObject player;
    public GameObject Player => player;

    [Header("Class Selection")]
    [SerializeField] private SOPlayerClass[] availableClasses;
    private SOPlayerClass chosenClass;
    private GameObject playerPrefab;

    public event System.Action<GameObject> OnPlayerSpawned;

    private void Awake()
    {
        leftTry = maxTry;

      
        if (Container.Resolver != null)
            Container.Resolver.RegisterInstance(this);
        else
            Debug.LogWarning("[RespawnManager] Container.Resolver non trovato.");

        FindRespawnPoint();
    }

    private void Start()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;

        if (chosenClass != null)
        {
            SetPlayerPrefab(chosenClass.prefab);
            SpawnPlayer();
        }
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;

        if (Container.Resolver != null)
            Container.Resolver.UnregisterInstance<RespawnManager>();
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindRespawnPoint();
        SpawnPlayer();
        FindPlayer();
    }

    private void FindRespawnPoint()
    {
        GameObject rp = GameObject.FindGameObjectWithTag("RespawnPoint");
        respawnPoint = rp != null ? rp.transform : null;
        if (respawnPoint == null)
            Debug.LogWarning("[RespawnManager] Nessun RespawnPoint trovato in questa scena.");
    }

    public void SelectClass(int index)
    {
        if (index < 0 || index >= availableClasses.Length)
        {
            Debug.LogWarning("[RespawnManager] Classe non valida!");
            return;
        }

        chosenClass = availableClasses[index];
        SetPlayerPrefab(chosenClass.prefab);
    }

    public void SetPlayerPrefab(GameObject prefab)
    {
        playerPrefab = prefab;
    }

    public void SpawnPlayer()
    {
        if (playerPrefab == null)
        {
            Debug.LogWarning("[RespawnManager] Player prefab non assegnato!");
            return;
        }

        if (player != null)
            Destroy(player);

        Vector3 spawnPos = respawnPoint != null ? respawnPoint.position + Vector3.up * 0.5f : Vector3.zero;
        player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);

        OnPlayerSpawned?.Invoke(player);
    }

    private void FindPlayer()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    public void ResetTries()
    {
        leftTry = maxTry;
    }

    public void PlayerDied()
    {
        leftTry--;

        if (leftTry > 0)
        {
            StartCoroutine(RespawnRoutine());
        }
        else
        {
            GameOver();
        }
    }

    private IEnumerator RespawnRoutine()
    {
        if (player == null) yield break;

        bool done = false;
        var fader = Container.Resolver.Resolve<ScreenFader>();
        fader?.FadeOut(() => done = true);
        while (!done) yield return null;

        Destroy(player);
        yield return new WaitForSeconds(respawnDelay);

        SpawnPlayer();

        done = false;
        fader?.FadeIn(() => done = true);
        while (!done) yield return null;
    }

    private void GameOver()
    {
        Debug.Log("[RespawnManager] GAME OVER");
        ResetTries();

        var fader = Container.Resolver.Resolve<ScreenFader>();
        fader?.FadeOut(() =>
        {
            SceneManager.LoadScene("StartMenu");
            fader?.FadeIn();
        });
    }

    public void SwitchPlayerPrefab(SOPlayerClass newClass)
    {
        if (newClass == null || newClass.prefab == null)
        {
            Debug.LogWarning("[RespawnManager] SwitchPlayerPrefab: classe o prefab non valida.");
            return;
        }

        Vector3 lastPos = player != null ? player.transform.position : (respawnPoint?.position ?? Vector3.zero);
        Quaternion lastRot = player != null ? player.transform.rotation : Quaternion.identity;

        if (player != null)
            Destroy(player);

        chosenClass = newClass;
        playerPrefab = newClass.prefab;

        player = Instantiate(playerPrefab, lastPos, lastRot);

        OnPlayerSpawned?.Invoke(player);
    }
}

