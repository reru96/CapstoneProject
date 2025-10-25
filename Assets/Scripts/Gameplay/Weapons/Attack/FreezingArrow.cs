using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.AI;

public class FreezingArrow : Arrow
{
    [SerializeField] private float freezeRadius = 3f;
    [SerializeField] private float freezeDuration = 2f;
    [SerializeField] private GameObject freezeEffectPrefab;

    protected override void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        if (!other.CompareTag("Enemy")) return;

        hasHit = true;

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        ApplyFreeze();
    }

    private void ApplyFreeze()
    {
        var pooler = ServiceLocator.Get<ObjectPooler>();

        if (freezeEffectPrefab != null && pooler != null)
        {
            var fx = pooler.Spawn<Poolable>(freezeEffectPrefab, transform.position, Quaternion.identity);
            fx?.gameObject.SetActive(true);
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, freezeRadius);
        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            var enemy = hit.GetComponent<EnemyStateMachine>();
            var agent = hit.GetComponent<NavMeshAgent>();
            if (enemy != null && agent != null)
            {
                agent.isStopped = true;
                StartCoroutine(UnfreezeAgent(agent, freezeDuration));
            }
        }

        GetComponent<Poolable>()?.ReturnToPool();
    }

    private IEnumerator UnfreezeAgent(NavMeshAgent agent, float duration)
    {
        yield return new WaitForSeconds(duration);
        if (agent != null)
            agent.isStopped = false;
    }
}
