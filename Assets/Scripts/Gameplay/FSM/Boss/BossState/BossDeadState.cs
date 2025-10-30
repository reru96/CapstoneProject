using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;

public class BossDeadState : BossBaseState
{
    public BossDeadState(BossStateMachine boss) : base(boss) { }

    public override void Enter()
    {
        boss.agent.isStopped = true;
        var col = boss.GetComponent<Collider>();
        if (col != null) col.enabled = false;
        CoinManager.Instance.AddCoins(boss.enemyData.coinReward);

        PlayAnim("Die");

        Object.Destroy(boss.gameObject, 10f);
    }

    public override void Exit() { }

    public override void Tick() { }
}
