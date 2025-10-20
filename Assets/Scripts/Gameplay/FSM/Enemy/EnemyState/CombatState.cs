using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using Random = UnityEngine.Random;
using UnityEngine;
using UnityEngine.AI;

public class CombatState : EnemyBaseState
{
    public CombatState(EnemyStateMachine enemy) : base(enemy) { }

    public int raysCount = 8;
    public float rayDistance = 5f;
    public bool drawDebugRays = true;

    public float stallingSpeed = 1.5f;
    public float stallingChangeDirTime = 1.5f;

    private float stallingTimer;
    private float stallingDirection = 1f;
 

    public override void Enter()
    {

        enemy.agent.isStopped = false;
        enemy.anim.SetFloat("Stalling", 1f);
        enemy.anim.SetFloat("MoveX", 0f);

        stallingTimer = Random.Range(1f, stallingChangeDirTime);
    }

    public override void Exit()
    {
        enemy.anim.SetFloat("Stalling", 0f);
        enemy.anim.SetFloat("MoveX", 0f);
    }

    public override void Tick()
    {
        int attackingEnemiesCount = CountAttackingEnemies();

        if (attackingEnemiesCount > 0)
        {
            PerformStallingMovement();
        }
        else
        {
            enemy.SwitchState(new AttackState(enemy));
        }
    }

    void PerformStallingMovement()
    {

        stallingTimer -= Time.deltaTime;
        if (stallingTimer <= 0f)
        {
            stallingDirection *= -1f;
            stallingTimer = Random.Range(1f, stallingChangeDirTime);
        }
        Vector3 toTarget = (enemy.targetPlayer.position - enemy.transform.position).normalized;
        Vector3 sideDir = Vector3.Cross(Vector3.up, toTarget) * stallingDirection;

        Vector3 stallingTarget = enemy.transform.position + sideDir * 1.5f;

        if (NavMesh.SamplePosition(stallingTarget, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            enemy.agent.SetDestination(hit.position);
        }

        enemy.anim.SetFloat("MoveX", Mathf.Lerp(enemy.anim.GetFloat("MoveX"), stallingDirection, Time.deltaTime * 3f));
    }

    int CountAttackingEnemies()
    {
        HashSet<GameObject> hitEnemies = new HashSet<GameObject>();

        for (int i = 0; i < raysCount; i++)
        {
            float angleDeg = i * 360f / raysCount;
            float angleRad = angleDeg * Mathf.Deg2Rad;
            Vector3 dir = new Vector3(Mathf.Cos(angleRad), 0, Mathf.Sin(angleRad));

            if (Physics.Raycast(enemy.transform.position, dir, out RaycastHit hit, rayDistance, enemy.otherEnemyMask))
            {
                if (drawDebugRays)
                    Debug.DrawRay(enemy.transform.position, dir * rayDistance, Color.green);

                GameObject go = hit.collider.gameObject;
                if (go != enemy.gameObject && go.CompareTag("Enemy"))
                {
                    var otherEnemy = go.GetComponent<EnemyStateMachine>();
                    if (otherEnemy != null && otherEnemy.CurrentState is AttackState)
                        hitEnemies.Add(go);
                }
            }
        }

        return hitEnemies.Count;
    }

}
