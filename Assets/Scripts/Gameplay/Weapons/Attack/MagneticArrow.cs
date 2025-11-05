using System.Collections;
using System.Collections.Generic;
using Core;
using GamePlay;
using UnityEngine;
using UnityEngine.AI;

public class MagneticArrow : Arrow
{

    [SerializeField] private float pullRadius = 2.5f;
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
        GameObject fxInstance = null;

        if (magneticEffectPrefab != null)
        {
            fxInstance = Instantiate(magneticEffectPrefab, transform.position, Quaternion.identity);

            fxInstance.transform.localScale = Vector3.one * pullRadius;
        }

        float elapsed = 0f;

        while (elapsed < pullDuration)
        {
            elapsed += Time.deltaTime;

            if (fxInstance != null)
                fxInstance.transform.position = transform.position;

            Collider[] hits = Physics.OverlapSphere(transform.position, pullRadius);
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
                        Vector3 dir = (transform.position - enemy.transform.position).normalized;
                        agent.Move(dir * pullForce * Time.deltaTime);
                    }
                    else
                    {
                        enemy.transform.position = Vector3.MoveTowards(enemy.transform.position,transform.position, pullForce * Time.deltaTime);
                        
                    }
                }
            }

            yield return null;
        }

        if (fxInstance != null)
            Destroy(fxInstance);

        if (poolable != null)
            poolable.ReturnToPool();
        else
            ServiceLocator.Get<ObjectPooler>()?.ReturnToPool(gameObject);
    }
}
