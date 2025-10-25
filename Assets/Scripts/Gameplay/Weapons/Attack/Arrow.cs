using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

public class Arrow : BaseAttack
{
    public float speed = 20f;
    protected Rigidbody rb;
    protected bool hasHit = false;

    protected override void OnEnable()
    {
        base.OnEnable();
        hasHit = false;

        if (rb == null)
            rb = GetComponent<Rigidbody>();

        rb.AddForce(transform.forward * speed, ForceMode.Impulse);
    }

    protected override void OnTriggerEnter(Collider other)
    {
        if (hasHit) return;
        if (!other.CompareTag("Enemy")) return;

        hasHit = true;

        var enemy = other.GetComponent<EnemyStateMachine>();
        if (enemy != null)
        {
            enemy.OnHit(transform.position);
            enemy.transform.position += transform.forward * repulseForce;
            DamageCalculation(enemy);
        }

        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        GetComponent<Poolable>()?.ReturnToPool();
    }
}
