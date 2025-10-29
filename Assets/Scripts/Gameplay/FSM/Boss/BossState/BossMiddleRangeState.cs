using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossMiddleRangeState : BossBaseState
{
    private float attackCooldown = 2f;    
    private float attackRange = 3f;       
    private float attackWindup = 0.5f;    
    private float timer;
    private bool isAttacking = false;

    public BossMiddleRangeState(BossStateMachine boss) : base(boss) { }

    public override void Enter()
    {
        timer = 0f;
        boss.agent.isStopped = false;
        boss.agent.speed = boss.chasingSpeed * 0.75f; 
        PlayAnim("Idle");
    }

    public override void Exit()
    {
        boss.agent.isStopped = false;
        isAttacking = false;
    }

    public override void Tick()
    {
        if (boss.targetPlayer == null) return;

        float distance = DistanceToPlayer();
        boss.transform.LookAt(boss.targetPlayer.position);

        if (distance > 6f)
        {
            boss.SwitchState(new BossApproachingState(boss));
            return;
        }

        if (distance <= 2f)
        {
            boss.SwitchState(new BossRangeState(boss));
            return;
        }

        if (!isAttacking)
            boss.agent.SetDestination(boss.targetPlayer.position);

        timer += Time.deltaTime;

        if (!isAttacking && timer >= attackCooldown && distance <= attackRange)
        {
            timer = 0f;
            boss.StartCoroutine(PerformMeleeAttack());
        }
    }

    private System.Collections.IEnumerator PerformMeleeAttack()
    {
        isAttacking = true;
        boss.agent.isStopped = true;
        PlayAnim("MiddleRange");

        yield return new WaitForSeconds(attackWindup);

        TryDealDamage();

        yield return new WaitForSeconds(0.8f);
        boss.agent.isStopped = false;
        isAttacking = false;
    }

    private void TryDealDamage()
    {
        if (boss.targetPlayer == null) return;

        float distance = DistanceToPlayer();
        if (distance <= attackRange + 0.5f)
        {
            var life = boss.targetPlayer.GetComponent<LifeController>();
            if (life != null)
            {
                float damage = DamageUtility.CalculateEnemyDamage(boss.enemyData, life.GetComponent<PlayerStats>());
                DamageUtility.ApplyDamageToPlayer(life.GetComponent<PlayerStats>(), boss.enemyData, damage);
            }

            if (boss.hitEffectPrefab != null)
            {
                Object.Instantiate(boss.hitEffectPrefab, boss.targetPlayer.position + Vector3.up * 1f, Quaternion.identity);
            }
        }
    }
}
