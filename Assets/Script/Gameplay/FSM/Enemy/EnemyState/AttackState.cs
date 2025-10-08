using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackState : EnemyBaseState
{
    private float attackRange = 2f;       
    private float attackCooldown = 1.5f;  
    private float lastAttackTime;

    public AttackState(EnemyStateMachine enemy) : base(enemy) { }

    public override void Enter()
    {
        enemy.anim.Play("Attack");
        lastAttackTime = -attackCooldown; 
        enemy.agent.isStopped = true;     
    }

    public override void Tick()
    {
        if (enemy.targetPlayer == null)
        {
            enemy.SwitchState(new PatrollingState(enemy));
            return;
        }

      
        Vector3 dirToPlayer = (enemy.targetPlayer.position - enemy.transform.position).normalized;
        dirToPlayer.y = 0;
        if (dirToPlayer.sqrMagnitude > 0.001f)
        {
            enemy.transform.rotation = Quaternion.Slerp(
                enemy.transform.rotation,
                Quaternion.LookRotation(dirToPlayer),
                Time.deltaTime * 5f
            );
        }

       
        float distToPlayer = Vector3.Distance(enemy.transform.position, enemy.targetPlayer.position);
        if (distToPlayer > attackRange)
        {
           
            enemy.agent.isStopped = false;
            enemy.SwitchState(new ChasingState(enemy));
            return;
        }

     
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            PerformAttack();
            lastAttackTime = Time.time;
        }
    }

    public override void Exit()
    {
        enemy.agent.isStopped = false;
    }

    private void PerformAttack()
    {
        LifeController playerHealth = enemy.targetPlayer.GetComponent<LifeController>();
        if (playerHealth != null)
        {
            playerHealth.AddHp(-1);
        }
    }
}
