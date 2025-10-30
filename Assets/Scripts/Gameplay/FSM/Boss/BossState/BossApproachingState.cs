using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossApproachingState : BossBaseState
{
    public BossApproachingState(BossStateMachine boss) : base(boss) { }

    public override void Enter()
    {
        boss.agent.speed = boss.chasingSpeed;
        boss.agent.stoppingDistance = 1f;
        boss.agent.SetDestination(boss.targetPlayer.position);
        PlayAnim("Run");
    }

    public override void Exit() { }

    public override void Tick()
    {
        if (boss.targetPlayer == null) return;

        boss.agent.SetDestination(boss.targetPlayer.position);

        float distance = DistanceToPlayer();
        if (distance <= 4f)
            boss.SwitchState(new BossMiddleRangeState(boss));
    }
}
