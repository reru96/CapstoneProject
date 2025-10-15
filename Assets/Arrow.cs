using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    [Header("Movement Settings")]
    public float speed = 20f;              
    public float destroyDelay = 3f;        
    public float repulseForce = 3f;        

    [Header("Damage Settings")]
    public int damage = 1;

    private Rigidbody rb;
    private bool hasHit = false;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;

        rb.AddForce(transform.forward * speed, ForceMode.VelocityChange);

     
        Destroy(gameObject, destroyDelay);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (hasHit) return; 
        if (!other.CompareTag("Enemy")) return;

        hasHit = true;

     
        var enemy = other.GetComponent<EnemyStateMachine>();
        var life = other.GetComponent<LifeController>();

        if (enemy != null)
        {
            enemy.OnHit(transform.position);
            enemy.transform.position += transform.forward * repulseForce; 
        }

        if (life != null)
        {
            life.AddHp(-damage);
        }
   
        Destroy(gameObject);
    }
}
