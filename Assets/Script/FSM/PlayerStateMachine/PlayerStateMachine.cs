using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.AI;

public class PlayerStateMachine : StateMachine
{
    public SOPlayerClass p_data;

    public SOWeapon weapon;

    public PlayerStats p_stats;
    public Animator animator {  get; set; }
    public Rigidbody rb { get; set; }
    public NavMeshAgent agent { get; set; }

    public float rotationSpeed = 25f;
  
    public bool isInvincible = false;

    public WeaponCombat weaponInstance;

    public Transform hand;

    void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        p_stats = GetComponent<PlayerStats>();

    }

    public void EquipWeapon(SOWeapon weaponData)
    {
        hand = p_data.prefab.transform.Find("mixamorig:RightHandMiddle4");
        var weaponObj = Instantiate(weaponData.prefab,hand.position,hand.localRotation);
        weaponObj.transform.SetParent(hand.transform,false);
   
        weaponInstance = weaponObj.GetComponent<WeaponCombat>();
        weaponInstance.Initialize(this);
    }

    public WeaponCombat GetWeapon() => weaponInstance;

    private void Start()
    {
        SwitchState(new PlayerIdleState(this));
        EquipWeapon(weapon);
    }

}
