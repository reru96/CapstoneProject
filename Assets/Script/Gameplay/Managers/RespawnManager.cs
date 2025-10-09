using System.Collections;
using UnityEngine;

[System.Serializable]
public class RespawnConfig
{
    public int maxTry = 3;
    public float respawnDelay = 2f;
}

public class RespawnManager : Injectable<RespawnManager>
{
    [SerializeField] private RespawnConfig config = new RespawnConfig();

    private int _remainingTries;
    private bool _isRespawning;

    public int RemainingTries => _remainingTries;
    public bool IsRespawning => _isRespawning;

    public event System.Action<int> OnTryCountChanged;
    public event System.Action OnRespawnStarted;
    public event System.Action OnRespawnCompleted;
    public event System.Action OnGameOver;

    protected override void OnInjected(ObjectResolver resolver)
    {
        base.OnInjected(resolver);
        _remainingTries = config.maxTry;
    }

    public void PlayerDied()
    {
        if (_isRespawning) return;

        _remainingTries--;
        OnTryCountChanged?.Invoke(_remainingTries);

        if (_remainingTries > 0)
            StartCoroutine(RespawnRoutine());
        else
            OnGameOver?.Invoke();
    }

    private IEnumerator RespawnRoutine()
    {
        _isRespawning = true;
        OnRespawnStarted?.Invoke();

        yield return new WaitForSeconds(config.respawnDelay);

        var classMgr = Resolve<ClassSelectionManager>();
        var spawnMgr = Resolve<PlayerSpawnManager>();
        if (classMgr.SelectedClass?.prefab != null)
            yield return spawnMgr.SpawnWhenReady(classMgr.SelectedClass.prefab);

        _isRespawning = false;
        OnRespawnCompleted?.Invoke();
    }

    public void ResetTries()
    {
        _remainingTries = config.maxTry;
        OnTryCountChanged?.Invoke(_remainingTries);
    }
}