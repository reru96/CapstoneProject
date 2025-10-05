using UnityEngine;
using System.Collections;
using UnityEditor.ProjectWindowCallback;

public class WaitingState : EnemyBaseState
{
    private float waitTime;

    public WaitingState(EnemyStateMachine enemy, float waitTime) : base(enemy)
    {
        this.waitTime = waitTime;
    }

    public override void Enter()
    {
        enemy.anim.Play("Idle");
        enemy.agent.isStopped = true;
        enemy.waitCoroutine = enemy.StartCoroutine(WaitCoroutine());
    }

    public override void Tick()
    {
       
        if (enemy.canSeePlayerNow)
        {
            
            enemy.StopCoroutine(enemy.waitCoroutine);
            enemy.agent.isStopped = false;
            enemy.SwitchState(new ChasingState(enemy));
        }
    }

    public override void Exit()
    {
        if (enemy.waitCoroutine != null)
        {
            enemy.StopCoroutine(enemy.waitCoroutine);
            enemy.waitCoroutine = null;
        }
        enemy.agent.isStopped = false;
    }

    private IEnumerator WaitCoroutine()
    {
        yield return new WaitForSeconds(waitTime);
        enemy.GoToNextWaypoint();
        enemy.SwitchState(new PatrollingState(enemy));
    }
}
