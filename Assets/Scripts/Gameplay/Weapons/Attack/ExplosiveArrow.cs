using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.AI;

public class ExplosiveArrow : Arrow
{

    [SerializeField] private float explosionRadius = 3f;
    [SerializeField] private float explosionForce = 5f;
    [SerializeField] private GameObject explosionEffect;

    private bool hasExploded = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        hasExploded = false;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (hasHit || hasExploded) return;
        if (!other.CompareTag("Enemy")) return;

        hasHit = true;
        hasExploded = true;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        Explode();
    }

    private void Explode()
    {
        var pooler = ServiceLocator.Get<ObjectPooler>();

        if (explosionEffect != null && pooler != null)
        {
            var fx = pooler.Spawn<Poolable>(explosionEffect, transform.position, Quaternion.identity);
            fx?.gameObject.SetActive(true);
        }

        Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
        foreach (var hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;

            var enemy = hit.GetComponent<EnemyStateMachine>();
            if (enemy != null)
            {
                float dmg = DamageUtility.CalculateDamage(shooterStats, weaponData, enemy.enemyData);
                DamageUtility.ApplyDamageToEnemy(enemy, dmg);

                var agent = hit.GetComponent<NavMeshAgent>();
                if (agent != null)
                {
                    Vector3 dir = (hit.transform.position - transform.position).normalized;
                    agent.Move(dir * explosionForce * Time.deltaTime);
                }
            }
        }

        if (poolable != null)
            poolable.ReturnToPool();
        else
            ServiceLocator.Get<ObjectPooler>()?.ReturnToPool(gameObject);
    }

}
