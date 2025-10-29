using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossRangeState : BossBaseState
{
    private float attackCooldown = 3f;
    private float timer;
    public GameObject projectilePrefab;
    private Transform firePoint;

    public BossRangeState(BossStateMachine boss) : base(boss)
    {
        firePoint = boss.eyes != null ? boss.eyes : boss.transform;
        projectilePrefab = boss.projectilePrefab; 
    }

    public override void Enter()
    {
        timer = 0f;
        boss.agent.isStopped = true;
        PlayAnim("Idle");
    }

    public override void Exit()
    {
        boss.agent.isStopped = false;
    }

    public override void Tick()
    {
        if (boss.targetPlayer == null) return;

        boss.transform.LookAt(boss.targetPlayer.position);
        timer += Time.deltaTime;

        float distance = DistanceToPlayer();

        if (distance > 10f)
            boss.SwitchState(new BossApproachingState(boss));
        else if (distance <= 4f)
            boss.SwitchState(new BossMiddleRangeState(boss));

        if (timer >= attackCooldown)
        {
            timer = 0f;
            PerformRangeAttack();
        }
    }

    private void PerformRangeAttack()
    {
        PlayAnim("RangeAttack");

        if (projectilePrefab == null) return;

        Vector3 spawnPos = firePoint.position + firePoint.forward * 1f;
        GameObject proj = Object.Instantiate(projectilePrefab, spawnPos, firePoint.rotation);

        if (proj.TryGetComponent(out BossProjectile projectile))
        {
            Vector3 dir = (boss.targetPlayer.position - firePoint.position).normalized;
            projectile.Launch(dir);
        }
    }
}
