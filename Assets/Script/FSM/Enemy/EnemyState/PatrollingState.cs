using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrollingState : EnemyBaseState
{
    public PatrollingState(EnemyStateMachine enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.anim.Play("Walk");
        enemy.agent.speed = enemy.patrollingSpeed;
        enemy.GoToNextWaypoint();
    }

    public override void Tick()
    {
        if (enemy.canSeePlayerNow)
        {
            enemy.SwitchState(new ChasingState(enemy));
            return;
        }

        if (!enemy.agent.pathPending && enemy.agent.remainingDistance <= enemy.agent.stoppingDistance)
        {
            enemy.SwitchState(new WaitingState(enemy, enemy.waitTimeAtWaypoint));
        }
    }

    public override void Exit()
    {
        if (enemy.waitCoroutine != null)
        {
            enemy.StopCoroutine(enemy.waitCoroutine);
            enemy.waitCoroutine = null;
        }
    }
}
