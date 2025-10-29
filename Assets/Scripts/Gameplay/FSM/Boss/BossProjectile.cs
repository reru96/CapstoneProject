using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

public class BossProjectile : MonoBehaviour
{

    public float speed = 10f;
    public float lifeTime = 5f;
    public float damage = 20f;
    public ParticleSystem impactEffect;

    private Rigidbody rb;
    private float timer;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        timer = 0f;
    }

    private void Update()
    {
        rb.velocity = transform.forward * speed;

        timer += Time.deltaTime;
        if (timer >= lifeTime)
        {
            ReturnToPool();
        }
    }

    public void Launch(Vector3 direction)
    {
        transform.forward = direction.normalized;
        rb.velocity = transform.forward * speed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (impactEffect)
        {
            var pooler = ServiceLocator.Get<ObjectPooler>();
            if (pooler != null)
                pooler.Spawn(impactEffect.gameObject, transform.position, Quaternion.identity);
            else
                Instantiate(impactEffect, transform.position, Quaternion.identity);
        }

        if (collision.gameObject.CompareTag("Player"))
        {
            var life = collision.gameObject.GetComponent<LifeController>();
            if (life != null)
                life.SetHp(life.GetHp() - Mathf.RoundToInt(damage));
        }

        ReturnToPool();
    }

    private void ReturnToPool()
    {
        var poolable = GetComponent<Poolable>();
        if (poolable != null)
        {
            poolable.SetReturnDelay(0f);
            poolable.ReturnToPool();
        }
        else
        {
            Destroy(gameObject);
        }
    }
}
