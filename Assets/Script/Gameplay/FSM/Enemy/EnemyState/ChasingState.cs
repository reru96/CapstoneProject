using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChasingState : EnemyBaseState
{
    public ChasingState(EnemyStateMachine enemy) : base(enemy) { }

    public float attackRange = 2;

    public override void Enter()
    {
        enemy.anim.Play("Run");
        enemy.agent.speed = enemy.chasingSpeed;
        enemy.agent.stoppingDistance = 1f; 
    }

    public override void Tick()
    {
        if (enemy.targetPlayer == null)
        {
            enemy.SwitchState(new PatrollingState(enemy));
            return;
        }

        float distToPlayer = Vector3.Distance(enemy.transform.position, enemy.targetPlayer.position);

        if (!enemy.canSeePlayerNow)
        {
            enemy.SwitchState(new AlertState(enemy));
            enemy.alertTimer = enemy.alertDuration;
            return;
        }

        if (distToPlayer <= attackRange)
        {
            enemy.SwitchState(new AttackState(enemy));
            return;
        }

        enemy.agent.SetDestination(enemy.targetPlayer.position);
    }

    public override void Exit() { }
}