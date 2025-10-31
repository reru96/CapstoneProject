using System.Collections;
using System.Collections.Generic;
using GamePlay;
using UnityEngine;

public class BossMeleeState : BossBaseState
{
    private bool isAttacking = false;
    private float attackRange = 1.5f;
    private float attackRecovery = 1.2f;

    public BossMeleeState(BossStateMachine boss) : base(boss)
    {
    }

    public override void Enter()
    {
        isAttacking = true;
        boss.agent.isStopped = true;
        boss.transform.LookAt(boss.targetPlayer.position);
        PlayAnim("Attack");

        TryDealDamage();

        boss.StartCoroutine(EndAttack());
    }

    public override void Exit()
    {
        boss.agent.isStopped = false;
        isAttacking = false;
    }

    public override void Tick()
    {
        if (boss.targetPlayer == null)
        {
            boss.SwitchState(new BossIdleState(boss));
            return;
        }

        float distance = DistanceToPlayer();

        if (!isAttacking && distance > attackRange + 0.5f)
        {
            boss.SwitchState(new BossApproachingState(boss));
        }
    }

    private System.Collections.IEnumerator EndAttack()
    {
        yield return new WaitForSeconds(attackRecovery);
        isAttacking = false;
        boss.SwitchState(new BossApproachingState(boss));
    }

    private void TryDealDamage()
    {
        if (boss.targetPlayer == null) return;

        float distance = DistanceToPlayer();
        if (distance <= attackRange)
        {
            var life = boss.targetPlayer.GetComponent<LifeController>();
            if (life != null)
            {
                float damage = DamageUtility.CalculateEnemyDamage(boss.enemyData, life.GetComponent<PlayerStats>());
                DamageUtility.ApplyDamageToPlayer(life.GetComponent<PlayerStats>(), boss.enemyData, damage);
            }

            if (boss.hitEffectPrefab != null)
            {
                Object.Instantiate(boss.hitEffectPrefab, boss.targetPlayer.position + Vector3.up, Quaternion.identity);
            }
        }
    }
}
