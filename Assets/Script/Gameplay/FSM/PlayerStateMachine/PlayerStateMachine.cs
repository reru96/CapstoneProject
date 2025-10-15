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
    public Animator animator { get; private set; }
    public Rigidbody rb { get; private set; }
    public NavMeshAgent agent { get; private set; }

    public float rotationSpeed = 25f;
    public bool isInvincible = false;

    private InventoryManager _inventory;
    [HideInInspector] public WeaponCombat weaponInstance;

    void Awake()
    {
        _inventory = CoreSystem.Instance.Container.Resolve<InventoryManager>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        p_stats = GetComponent<PlayerStats>();
        weaponInstance = GetComponentInChildren<WeaponCombat>();

        if (weaponInstance != null)
            weaponInstance.Initialize(this);
    }

    private void Start()
    {
        _inventory.runInventory.AddItem(weapon);
        p_stats.EquipWeapon(weapon);
        SwitchState(new PlayerIdleState(this));
    }

    public WeaponCombat GetWeapon() => weaponInstance;

}
