using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDeadState : EnemyBaseState
{

    public EnemyDeadState(EnemyStateMachine enemy) : base(enemy)
    {
    }

    public override void Enter()
    {
        if (enemy.agent != null)
            enemy.agent.isStopped = true;

        if (enemy.anim != null)
            enemy.anim.Play("Die");

        PlayerStats player = Object.FindObjectOfType<PlayerStats>();
        if (player != null && enemy.enemyData != null)
        {
            player.AddExperience(Mathf.RoundToInt(enemy.enemyData.expDrop));
        }

        var poolable = enemy.GetComponent<Poolable>();
        if (poolable != null)
        {
            poolable.SetReturnDelay(0f); 
            poolable.ReturnToPool();
        }
        else
        {
            Object.Destroy(enemy.gameObject);
        }
    }

    public override void Exit()
    {
       
    }

    public override void Tick()
    {

    }
}
