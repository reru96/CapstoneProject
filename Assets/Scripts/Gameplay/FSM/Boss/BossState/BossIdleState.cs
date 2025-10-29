using UnityEngine;

public class BossIdleState : BossBaseState
{
    private float timer = 0f;
    private float idleDuration = 2f;

    public BossIdleState(BossStateMachine boss) : base(boss) {}

    public override void Enter()
    {
        timer = 0f;
        boss.agent.isStopped = true;
        PlayAnim("Idle");
    }

    public override void Exit()
    {
        boss.agent.isStopped = false;
    }

    public override void Tick()
    {
        if (boss.targetPlayer == null) return;

        boss.transform.LookAt(boss.targetPlayer.position);

        timer += Time.deltaTime;
        if (timer < idleDuration) return;

        float distance = DistanceToPlayer();

        if (distance > 8f)
            boss.SwitchState(new BossApproachingState(boss));
        else if (distance > 4f)
            boss.SwitchState(new BossMiddleRangeState(boss));
        else
            boss.SwitchState(new BossRangeState(boss));
    }
}