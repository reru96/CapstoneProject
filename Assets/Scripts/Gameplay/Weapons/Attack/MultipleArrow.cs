using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEditor.EditorTools;
using UnityEngine;

public class MultipleArrow : Arrow
{
    [SerializeField] private Transform firePoint;
    [SerializeField] private float angle = 30f;
    [SerializeField] private int numberOfArrows = 3;
    [SerializeField] private GameObject arrowPrefab;

    protected override void OnEnable()
    {
        base.OnEnable();

        int middle = numberOfArrows / 2;
        for (int i = -middle; i <= middle; i++)
        {
            Shoot(i * angle);
        }

        if (poolable != null)
            poolable.ReturnToPool();
    }

    private void Shoot(float angleOffset)
    {
        var pooler = ServiceLocator.Get<ObjectPooler>();
        if (pooler == null || arrowPrefab == null) return;

        Quaternion rot = Quaternion.AngleAxis(angleOffset, Vector3.up);
        Vector3 dir = (rot * firePoint.forward).normalized;

        Arrow arrow = pooler.Spawn<Arrow>(arrowPrefab, firePoint.position, Quaternion.LookRotation(dir));
        if (arrow == null) return;

        arrow.Initialize(shooterStats, weaponData);

        if (arrow.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.AddForce(dir * speed, ForceMode.Impulse);
        }

        if (arrow.TryGetComponent<Poolable>(out var poolable))
            poolable.SetReturnDelay(lifeTime);
    }
}
