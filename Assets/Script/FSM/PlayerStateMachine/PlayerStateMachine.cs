using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

public class PlayerStateMachine : StateMachine
{
    public Animator animator {  get; set; }
    public Rigidbody rb { get; set; }
    public NavMeshAgent agent { get; set; }

    public float rotationSpeed = 25f;
  
    
    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();

    }

    private void Start()
    {
        SwitchState(new PlayerIdleState(this));
    }

}
