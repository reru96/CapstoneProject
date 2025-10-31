using System.Collections;
using System.Collections.Generic;
using Core;
using GamePlay;
using UnityEngine;

public class Projectile : MonoBehaviour
{

    public float speed = 15f;
    public float lifeTime = 5f;
    protected PlayerStats shooterStats;
    protected SOWeapon weaponData;
    protected float spawnTime;
    protected Poolable poolable;

    protected virtual void Awake()
    {
        poolable = GetComponent<Poolable>();
    }

    public virtual void Initialize(PlayerStats stats, SOWeapon weapon)
    {
        shooterStats = stats;
        weaponData = weapon;
        spawnTime = Time.time;

        if (poolable != null)
            poolable.SetReturnDelay(lifeTime);
    }

    protected virtual void Update()
    {
        transform.position += transform.forward * speed * Time.deltaTime;

        if (poolable == null && Time.time - spawnTime >= lifeTime)
        {
            ServiceLocator.Get<ObjectPooler>()?.ReturnToPool(gameObject);
        }
    }

    protected virtual void OnEnable()
    {
        spawnTime = Time.time;
        if (poolable != null)
            poolable.SetReturnDelay(lifeTime);
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        Collider thisCollider = GetComponent<Collider>();
        Collider[] enemies = null;

        int enemyLayerMask = LayerMask.GetMask("Enemy");

        if (thisCollider is SphereCollider sphere)
        {
            Vector3 center = transform.TransformPoint(sphere.center);
            float radius = sphere.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.y, transform.lossyScale.z);
            enemies = Physics.OverlapSphere(center, radius, enemyLayerMask);
        }
        else if (thisCollider is BoxCollider box)
        {
            Vector3 center = transform.TransformPoint(box.center);
            Vector3 halfExtents = Vector3.Scale(box.size / 2f, transform.lossyScale);
            enemies = Physics.OverlapBox(center, halfExtents, transform.rotation, enemyLayerMask);
        }
        else if (thisCollider is CapsuleCollider capsule)
        {
            Vector3 center = transform.TransformPoint(capsule.center);
            float radius = capsule.radius * Mathf.Max(transform.lossyScale.x, transform.lossyScale.z);
            float height = capsule.height * transform.lossyScale.y;

            Vector3 dir = Vector3.up; 
            if (capsule.direction == 0) dir = Vector3.right;
            else if (capsule.direction == 2) dir = Vector3.forward;

            Vector3 p1 = center + dir * (height / 2f - radius);
            Vector3 p2 = center - dir * (height / 2f - radius);

            enemies = Physics.OverlapCapsule(p1, p2, radius, enemyLayerMask);
        }

        if (enemies != null)
        {
            foreach (var hit in enemies)
            {
                if (hit.TryGetComponent<EnemyStateMachine>(out var enemy))
                {
                    enemy.OnHit(transform.position);
                    float dmg = DamageUtility.CalculateDamage(shooterStats, weaponData, enemy.enemyData);
                    DamageUtility.ApplyDamageToEnemy(enemy, dmg);
                }
            }
        }

        if (poolable != null)
            poolable.ReturnToPool();
        else
            ServiceLocator.Get<ObjectPooler>()?.ReturnToPool(gameObject);
    }
}
