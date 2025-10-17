using UnityEngine;
using UnityEngine.SceneManagement;
using Core;
using Gameplay;

public class LifeController : MonoBehaviour
{
    [SerializeField] private int maxHp = 100;
    [SerializeField] private int currentHp;
    [SerializeField] private bool fullHpOnAwake = true;
    [SerializeField] private DeathAction death = DeathAction.Destroy;

    public int GetMaxHp() => maxHp;
    public int GetHp() => currentHp;

    public enum DeathAction { None, Destroy, Disable, Die, SceneReload }

    private void Awake()
    {
        if (fullHpOnAwake)
            SetHp(maxHp);
    }

    public void SetHp(int hp)
    {
        int oldHp = currentHp;
        currentHp = Mathf.Clamp(hp, 0, maxHp);

        if (oldHp > 0 && currentHp == 0)
        {
            HandleDeath();
        }
    }

    public void AddHp(int amount) => SetHp(currentHp + amount);

    private void HandleDeath()
    {
        switch (death)
        {
            case DeathAction.None:
                break;

            case DeathAction.Destroy:
                Destroy(gameObject);
                break;

            case DeathAction.Disable:
                gameObject.SetActive(false);
                break;

            case DeathAction.Die:
                var respawnMgr = ServiceLocator.Get<RespawnManager>();
                if (respawnMgr != null)
                {
                    respawnMgr.RespawnPlayerAtCurrent();
                }
                else
                {
                    Debug.LogWarning("[LifeController] RespawnManager non trovato!");
                }
                break;

            case DeathAction.SceneReload:
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
        }
    }
}