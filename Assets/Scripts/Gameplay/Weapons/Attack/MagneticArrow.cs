using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.AI;

public class MagneticArrow : Arrow
{

    [SerializeField] private float pullRadius = 5f;
    [SerializeField] private float pullForce = 10f;
    [SerializeField] private float pullDuration = 1.5f;
    [SerializeField] private GameObject magneticEffectPrefab;

    private bool hasActivated = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        hasActivated = false;
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (hasHit || hasActivated) return;
        if (!other.CompareTag("Enemy")) return;

        hasHit = true;
        hasActivated = true;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        StartCoroutine(ActivateMagneticField());
    }

    private IEnumerator ActivateMagneticField()
    {
        var pooler = ServiceLocator.Get<ObjectPooler>();

        if (magneticEffectPrefab != null && pooler != null)
        {
            var fx = pooler.Spawn<Poolable>(magneticEffectPrefab, transform.position, Quaternion.identity);
            fx?.gameObject.SetActive(true);
        }

        float elapsed = 0f;
        Vector3 center = transform.position;

        while (elapsed < pullDuration)
        {
            elapsed += Time.deltaTime;

            Collider[] hits = Physics.OverlapSphere(center, pullRadius);
            foreach (var hit in hits)
            {
                if (!hit.CompareTag("Enemy")) continue;

                var enemy = hit.GetComponent<EnemyStateMachine>();
                var agent = hit.GetComponent<NavMeshAgent>();
                if (enemy != null)
                {
           
                    float dmg = DamageUtility.CalculateDamage(shooterStats, weaponData, enemy.enemyData);
                    DamageUtility.ApplyDamageToEnemy(enemy, dmg);

                    if (agent != null)
                    {
                        Vector3 dir = (center - enemy.transform.position).normalized;
                        agent.Move(dir * pullForce * Time.deltaTime);
                    }
                }
            }

            yield return null;
        }

        if (poolable != null)
            poolable.ReturnToPool();
        else
            ServiceLocator.Get<ObjectPooler>()?.ReturnToPool(gameObject);
    }
}
