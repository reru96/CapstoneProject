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
    private int _upperBodyLayerIndex;
    public bool isInvincible = false;

    [HideInInspector] public WeaponCombat weaponInstance;
    private InventoryManager _inventory;

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
        int upperBodyLayer = animator.GetLayerIndex("UpperBody");
        animator.SetLayerWeight(upperBodyLayer, 0f);
    }

    public void SetUpperBodyActive(bool active)
    {
        float targetWeight = active ? 1f : 0f;
        animator.SetLayerWeight(_upperBodyLayerIndex, targetWeight);
    }

    private void Start()
    {
      
        if (weapon != null)
        {
            _inventory.runInventory.AddItem(weapon);
            _inventory.runInventory.EquipItem(weapon);
            p_stats.EquipWeapon(weapon);
        }

        SwitchState(new PlayerIdleState(this));
    }

    public WeaponCombat GetWeapon() => weaponInstance;

    public void EquipNewWeapon(SOWeapon newWeapon)
    {
        if (newWeapon == null) return;

        weapon = newWeapon;
        p_stats.EquipWeapon(newWeapon);

        Debug.Log($"[PlayerStateMachine] Equipped new weapon: {newWeapon.name}");
    }

}
