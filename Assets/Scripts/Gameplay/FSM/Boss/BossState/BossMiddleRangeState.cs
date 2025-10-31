using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Core;
using Gameplay;
using GamePlay;

public class BossMiddleRangeState : BossBaseState
{
    private float attackRange = 1f;
    private bool isAttacking = false;

    public BossMiddleRangeState(BossStateMachine boss) : base(boss) { }

    public override void Enter()
    {
        isAttacking = true;
        boss.agent.isStopped = true;   
        boss.transform.LookAt(boss.targetPlayer.position);
        PlayAnim("MiddleRange");

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
    }

    private System.Collections.IEnumerator EndAttack()
    {
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
