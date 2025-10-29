using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossStateMachine : EnemyStateMachine
{
    public GameObject projectilePrefab;

    protected  void Start()
    {
        SwitchState(new BossIdleState(this));
    }

    private void OnEnable()
    {
        if (life != null)
            GameEvent.OnBossDead += OnBossDead;
    }

    private void OnDisable()
    {
        if (life != null)
            GameEvent.OnBossDead -= OnBossDead;
    }

    private void OnBossDead()
    {
        SwitchState(new BossDeadState(this));
    }
}
