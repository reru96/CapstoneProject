using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossApproachingState : BossBaseState
{
    public BossApproachingState(BossStateMachine boss) : base(boss) { }

    public override void Enter()
    {
        boss.agent.isStopped = false;
        boss.agent.speed = boss.chasingSpeed;
        boss.agent.stoppingDistance = 1f;
        PlayAnim("Run");
    }

    public override void Exit()
    {
        boss.agent.isStopped = true;
    }

    public override void Tick()
    {
        if (boss.targetPlayer == null)
        {
            boss.SwitchState(new BossIdleState(boss));
            return;
        }

        boss.agent.SetDestination(boss.targetPlayer.position);

        float distance = DistanceToPlayer();
        if (distance <= boss.agent.stoppingDistance + 0.1f)
        {
            boss.SwitchState(new BossMeleeState(boss));
        }
    }
}
