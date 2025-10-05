using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : MonoBehaviour
{
    public float distance = 1f;     
    public float duration = 0.2f;   
    public float destroyDelay = 0.1f;

    private Vector3 startPos;
    private Vector3 endPos;
    private float elapsed = 0f;

    void Start()
    {
        startPos = transform.position;
        endPos = startPos + transform.forward * distance;
    }

    void Update()
    {
        if (elapsed < duration)
        {
            
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;

            if (elapsed >= duration)
            {
                transform.position = endPos;
                Destroy(gameObject, destroyDelay); 
            }
        }
    }

  
    private void OnTriggerEnter(Collider other)
    {
       
        if (other.CompareTag("Enemy"))
        {
            var life = other.GetComponent<LifeController>();
            life.AddHp(-1);
        }
    }
}
