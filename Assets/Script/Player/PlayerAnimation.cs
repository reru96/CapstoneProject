using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlayerAnimation : MonoBehaviour
{
    private NavMeshAgent agent;
    private Animator anim;

    void Start()
    {
     agent = GetComponent<NavMeshAgent>();  
     anim = GetComponent<Animator>();
    }

    private void Update()
    {
        UpdateMovementAnimation();
    }

    private void UpdateMovementAnimation()
    {
        float speed = agent.velocity.magnitude;
        anim.SetFloat("Speed", speed);
        if(Input.GetMouseButtonDown(0))
        {
            agent.speed = 0;
            anim.SetTrigger("Attack");
        }

        if (Input.GetMouseButtonDown(1)) 
        {
            anim.SetTrigger("Dodge");
        }
    }
}
