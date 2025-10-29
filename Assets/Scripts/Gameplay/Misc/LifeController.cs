using UnityEngine;
using UnityEngine.SceneManagement;
using Core;
using Gameplay;

public class LifeController : MonoBehaviour
{
    [SerializeField] private int maxHp = 100;
    [SerializeField] private int currentHp;
    [SerializeField] private bool fullHpOnAwake = true;
    [SerializeField] private DeathAction death = DeathAction.Disable;

    public int GetMaxHp() => maxHp;
    public int GetHp() => currentHp;
    public DeathAction Death => death;

    public enum DeathAction { None, Destroy, Disable, Die, SceneReload, playerDead, BossDead, EnemyDead }

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
            case DeathAction.SceneReload:
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
                break;
            case DeathAction.playerDead:
                GameEvent.PlayerDead();
                break;
            case DeathAction.BossDead:
                GameEvent.BossDead();
                break;
            case DeathAction.EnemyDead:
                var enemySM = GetComponent<EnemyStateMachine>();
                if (enemySM != null)
                {
                    enemySM.SwitchState(new EnemyDeadState(enemySM));
                }
                break;  
        }
    }
}