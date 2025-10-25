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

    protected override void OnEnable()
    {
        base.OnEnable();

        int middle = numberOfArrows / 2;
        for (int i = -middle; i <= middle; i++)
        {
            Shoot(i * angle);
        }
    }

    private void Shoot(float angleOffset)
    {
        var pooler = ServiceLocator.Get<ObjectPooler>();
        if (pooler == null) return;

        Quaternion rot = Quaternion.AngleAxis(angleOffset, Vector3.up);
        Vector3 dir = (rot * firePoint.forward).normalized;

        Arrow arrow = pooler.Spawn<Arrow>(gameObject, firePoint.position, Quaternion.LookRotation(dir));
        if (arrow == null) return;

        if (arrow.TryGetComponent<Rigidbody>(out var rb))
        {
            rb.velocity = dir * speed;
            rb.AddForce(dir * speed, ForceMode.Impulse);
        }

        if (arrow.TryGetComponent<Poolable>(out var poolable))
            poolable.SetReturnDelay(destroyDelay);
    }
}
