using UnityEngine;

public class BossIdleState : BossBaseState
{
    public BossIdleState(BossStateMachine boss) : base(boss) { }

    public float maxDistance = 10f;
    public override void Enter()
    {
        boss.agent.isStopped = true;
        PlayAnim("Idle");
    }

    public override void Exit() { }

    public override void Tick()
    {
        if (boss.targetPlayer == null) return;

        float distance = DistanceToPlayer();
        if (distance < maxDistance) 
        {
            boss.SwitchState(new BossApproachingState(boss));
        }
    }
}