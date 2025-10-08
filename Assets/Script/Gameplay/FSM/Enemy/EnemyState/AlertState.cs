using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlertState : EnemyBaseState
{
    public AlertState(EnemyStateMachine enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.anim.Play("Walk");
        enemy.agent.speed = enemy.alertSpeed;
        enemy.agent.stoppingDistance = 0f;
        if (enemy.lastSeenPosition != Vector3.zero)
            enemy.agent.SetDestination(enemy.lastSeenPosition);
    }

    public override void Tick()
    {
        if (enemy.canSeePlayerNow)
        {
            enemy.lastSeenPosition = enemy.targetPlayer.position;
            enemy.SwitchState(new ChasingState(enemy));
            return;
        }

        if (enemy.agent.pathPending) return;

        if (!enemy.agent.pathPending && enemy.agent.remainingDistance <= enemy.agent.stoppingDistance + 0.2f)
        {
            enemy.alertTimer -= Time.deltaTime;

            if (enemy.alertTimer <= 0f)
            {
                enemy.SwitchState(new PatrollingState(enemy));
                enemy.GoToNewDynamicGoal();
            }
        }
    }

    public override void Exit()
    {

    }
}
