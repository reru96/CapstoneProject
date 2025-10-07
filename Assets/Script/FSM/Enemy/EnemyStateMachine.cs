using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.ProBuilder.Shapes;

public class EnemyStateMachine : StateMachine
{
    [Header("Riferimenti")]
    public Transform targetPlayer;
    public NavMeshAgent agent;
    public Transform eyes;
    public Animator anim;
    public ParticleSystem hitEffectPrefab;

    [Header("Patrolling")]
    public float patrolRadius = 10f;
    public float waitTimeAtGoal = 2f;
    public Coroutine waitCoroutine;

    [Header("Velocità")]
    public float patrollingSpeed = 1.5f;
    public float chasingSpeed = 3.5f;
    public float alertSpeed = 1.5f;

    [Header("Visione")]
    public float viewRadius = 12f;
    [Range(1f, 360f)]
    public float viewAngle = 90f;
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    public float checkInterval = 0.1f;
    public bool canSeePlayerNow;
    public float nextCheckTime;
    public Vector3 lastSeenPosition;
    public float lastSeenTime;

    [Header("Alert")]
    public float alertDuration = 5f;
    public float alertTimer;

    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
    }

    private void Start()
    {
        SwitchState(new PatrollingState(this));
    }

    protected override void Update()
    {
        base.Update();

        if (Time.time >= nextCheckTime)
        {
            nextCheckTime = Time.time + checkInterval;
            canSeePlayerNow = CanSeePlayer();

            if (canSeePlayerNow && targetPlayer != null)
            {
                lastSeenTime = Time.time;
                lastSeenPosition = targetPlayer.position;
            }
        }
    }

    public bool CanSeePlayer()
    {
        if (targetPlayer == null) return false;

        Vector3 eyePos = eyes ? eyes.position : transform.position + Vector3.up * 1.6f;
        Vector3 toTarget = targetPlayer.position - eyePos;
        float distToTarget = toTarget.magnitude;

        if (distToTarget > viewRadius) return false;

        Vector3 dirToTarget = toTarget.normalized;
        float angleToTarget = Vector3.Angle(transform.forward, dirToTarget);
        if (angleToTarget > viewAngle * 0.5f) return false;

        if (Physics.Raycast(eyePos, dirToTarget, out RaycastHit hit, distToTarget, obstructionMask))
            return false;

        return true;
    }

   
    public Vector3 GetRandomPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += transform.position;

        if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRadius, NavMesh.AllAreas))
            return hit.position;

        return transform.position;
    }

    public void GoToNewDynamicGoal()
    {
        Vector3 newGoal = GetRandomPatrolPoint();
        agent.speed = patrollingSpeed;
        agent.stoppingDistance = 0f;
        agent.isStopped = false;
        agent.SetDestination(newGoal);
    }

    public IEnumerator WaitAndPatrolAgain()
    {
        yield return new WaitForSeconds(waitTimeAtGoal);
        GoToNewDynamicGoal();
        waitCoroutine = null;
    }

    public void OnHit(Vector3 hitPosition)
    {
       
        if (!(CurrentState is HitState))
        {
        
            lastSeenPosition = hitPosition;
            SwitchState(new HitState(this));
        }
    }

}
