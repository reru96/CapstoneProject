using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossHitState : BossBaseState
{
    private float hitDuration = 0.5f;
    private float timer;

    public BossHitState(BossStateMachine boss) : base(boss) { }

    public override void Enter()
    {
        timer = 0f;
        boss.agent.isStopped = true;
        PlayAnim("Hit");
    }

    public override void Exit()
    {
        boss.agent.isStopped = false;
    }

    public override void Tick()
    {
        timer += Time.deltaTime;
        if (timer >= hitDuration)
            boss.SwitchState(new BossIdleState(boss));
    }
}
