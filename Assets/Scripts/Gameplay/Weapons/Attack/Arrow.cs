using System.Collections;
using System.Collections.Generic;
using Codice.CM.Common;
using Core;
using UnityEngine;

public class Arrow : Projectile
{
    public float repulseForce = 1f;
    protected Rigidbody rb;
    protected bool hasHit = false;

    protected override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        hasHit = false;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(transform.forward * speed, ForceMode.Impulse);
        }
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        if (!other.CompareTag("Enemy")) return;

        hasHit = true;

        if (other.TryGetComponent<EnemyStateMachine>(out var enemy))
        {
            enemy.OnHit(transform.position);
            enemy.transform.position += transform.forward * repulseForce;

            float dmg = DamageUtility.CalculateDamage(shooterStats, weaponData, enemy.enemyData);
            DamageUtility.ApplyDamageToEnemy(enemy, dmg);
        }

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (poolable != null)
            poolable.ReturnToPool();
        else
            ServiceLocator.Get<ObjectPooler>()?.ReturnToPool(gameObject);
    }
}