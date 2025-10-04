using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RespawnManager : Singleton<RespawnManager> 
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

    protected override bool ShouldBeDestroyOnLoad() => false;
    public event System.Action<GameObject> OnPlayerSpawned;

    protected override void Awake()
    {
        base.Awake();
        leftTry = maxTry;
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

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        FindRespawnPoint();
        SpawnPlayer();
        FindPlayer();
        FindObjectOfType<UILives>()?.UpdateLives();
    }

    private void FindRespawnPoint()
    {
        GameObject rp = GameObject.FindGameObjectWithTag("RespawnPoint");
        respawnPoint = rp != null ? rp.transform : null;
        if (respawnPoint == null)
            Debug.Log("RespawnManager: Nessun RespawnPoint trovato in questa scena.");
    }


    public void SelectClass(int index)
    {
        if (index < 0 || index >= availableClasses.Length)
        {
            Debug.LogWarning("Classe non valida!");
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
            Debug.LogWarning("Player prefab non assegnato!");
            return;
        }

        if (player != null)
            Destroy(player);

        player = Instantiate(playerPrefab, respawnPoint.position + Vector3.up * 0.5f, Quaternion.identity);

        OnPlayerSpawned?.Invoke(Player);
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
        FindObjectOfType<UILives>()?.UpdateLives();

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
        ScreenFader.Instance.FadeOut(() => done = true);
        while (!done) yield return null;

        Destroy(player);
        yield return new WaitForSeconds(respawnDelay);

        SpawnPlayer();

        done = false;
        ScreenFader.Instance.FadeIn(() => done = true);
        while (!done) yield return null;
    }

    private void GameOver()
    {
        Debug.Log("GAME OVER");
        ResetTries();
        ScreenFader.Instance.FadeOut(() =>
        {
            SceneManager.LoadScene("StartMenu");
            ScreenFader.Instance.FadeIn();
        });
    }
}

