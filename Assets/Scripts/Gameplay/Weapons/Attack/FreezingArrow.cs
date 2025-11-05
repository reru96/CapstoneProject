using System.Collections;
using System.Collections.Generic;
using Core;
using GamePlay;
using UnityEngine;
using UnityEngine.AI;

public class FreezingArrow : Arrow
{
    [SerializeField] private float freezeRadius = 3f;
    [SerializeField] private float freezeDuration = 2f;
    [SerializeField] private GameObject freezeAreaEffectPrefab;  
    [SerializeField] private GameObject freezeHitEffectPrefab;   

    protected override void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        if (!other.CompareTag("Enemy")) return;

        hasHit = true;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        StartCoroutine(ApplyFreeze());
    }

    private IEnumerator ApplyFreeze()
    {
        if (freezeAreaEffectPrefab != null)
        {
            GameObject fx = Instantiate(freezeAreaEffectPrefab, transform.position, Quaternion.identity);
            fx.transform.localScale = Vector3.one * freezeRadius;
            Destroy(fx, freezeDuration);
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, freezeRadius);
        foreach (Collider hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            var enemy = hit.GetComponent<EnemyStateMachine>();
            var agent = hit.GetComponent<NavMeshAgent>();

            if (enemy != null)
            {
                float dmg = DamageUtility.CalculateDamage(shooterStats, weaponData, enemy.enemyData);
                DamageUtility.ApplyDamageToEnemy(enemy, dmg);

                if (freezeHitEffectPrefab != null)
                {
                    GameObject hitFx = Instantiate(freezeHitEffectPrefab, enemy.transform.position, Quaternion.identity, enemy.transform);
                    Destroy(hitFx, freezeDuration);
                }

                if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
                {
                    agent.isStopped = true;
                    StartCoroutine(UnfreezeAgent(agent, freezeDuration));
                }
                else
                {
                    StartCoroutine(FreezeTransform(enemy.transform));
                }
            }
        }

        if (poolable != null)
            poolable.ReturnToPool();
        else
            ServiceLocator.Get<ObjectPooler>()?.ReturnToPool(gameObject);

        yield return null;
    }

    private IEnumerator UnfreezeAgent(NavMeshAgent agent, float duration)
    {
        yield return new WaitForSeconds(duration);

        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
            agent.isStopped = false;
    }

    private IEnumerator FreezeTransform(Transform enemy)
    {
        Vector3 pos = enemy.position;
        float timer = freezeDuration;

        while (timer > 0f)
        {
            enemy.position = pos;
            timer -= Time.deltaTime;
            yield return null;
        }
    }
}
