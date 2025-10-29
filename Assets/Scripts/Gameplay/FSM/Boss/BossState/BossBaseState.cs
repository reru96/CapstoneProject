using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BossBaseState : EnemyBaseState
{
    protected BossStateMachine boss;

    public BossBaseState(BossStateMachine boss) : base(boss)
    {
        this.boss = boss;
    }

    public override void Enter()
    {
        
    }

    public override void Exit()
    {
       
    }

    public override void Tick()
    {
        
    }

    protected float DistanceToPlayer()
    {
        if (boss.targetPlayer == null) return Mathf.Infinity;
        return Vector3.Distance(boss.transform.position, boss.targetPlayer.position);
    }

    protected void PlayAnim(string trigger)
    {
        if (boss.anim != null)
            boss.anim.Play(trigger);
    }

}
