using System.Collections;
using System.Collections.Generic;
using Core;
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

        if (other.TryGetComponent<EnemyStateMachine>(out var enemy))
        {
            enemy.OnHit(transform.position);
            float dmg = DamageUtility.CalculateDamage(shooterStats, weaponData, enemy.enemyData);
            DamageUtility.ApplyDamageToEnemy(enemy, dmg);
        }

        if (poolable != null)
            poolable.ReturnToPool();
        else
            ServiceLocator.Get<ObjectPooler>()?.ReturnToPool(gameObject);
    }
}
