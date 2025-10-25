using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Core;
using Gameplay;
using UnityEngine.AI;

public class PlayerStateMachine : StateMachine
{
    public SOPlayerClass p_data;
    public SOWeapon weapon;

    public PlayerStats p_stats;
    public Animator animator { get; set; }
    public Rigidbody rb { get; private set; }
    public NavMeshAgent agent { get; private set; }

    [HideInInspector] public Vector3 currentVelocity = Vector3.zero;   

    [SerializeField] private string upperBodyLayerName = "UpperBody";
    private int _upperBodyLayerIndex = -1;
    public int UpperBodyLayerIndex => _upperBodyLayerIndex;

    public float accelerationTime = 0.2f;
    public float rotationSpeed = 25f;
 
    public bool isInvincible = false;
    public bool isMoving = false;
    public bool isDodging = false;
    public bool isAttacking = false;

    [HideInInspector] public WeaponCombat weaponInstance;
    private InventoryManager _inventory;

    void Awake()
    {
        _inventory = ServiceLocator.Get<InventoryManager>();

        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        agent = GetComponent<NavMeshAgent>();
        p_stats = GetComponent<PlayerStats>();
        weaponInstance = GetComponentInChildren<WeaponCombat>();

        if (weaponInstance != null)
            weaponInstance.Initialize(this);
      
        if (animator != null)
        {
            _upperBodyLayerIndex = animator.GetLayerIndex(upperBodyLayerName);
            if (_upperBodyLayerIndex < 0)
                Debug.LogWarning($"[PlayerStateMachine] Layer '{upperBodyLayerName}' non trovato. Verifica il nome nel Animator.");
            else
                animator.SetLayerWeight(_upperBodyLayerIndex, 0f);
        }

    }

    private void Start()
    {
        if (weapon != null)
        {
            _inventory.runInventory.AddItem(weapon);
            _inventory.runInventory.EquipItem(weapon);
            if (p_stats != null)
                p_stats.EquipWeapon(weapon);
            
            var pooler = ServiceLocator.Get<ObjectPooler>();
            if (pooler != null)
            {
                pooler.ClearAllPools(); 
                pooler.ConfigurePoolsForWeapons(new List<SOWeapon> { weapon });
            }


            animator.runtimeAnimatorController = weapon.animator;
        }

        SwitchState(new PlayerIdleState(this));
    }

    public WeaponCombat GetWeapon() => weaponInstance;

    public void EquipNewWeapon(SOWeapon newWeapon)
    {
        if (newWeapon == null) return;

        weapon = newWeapon;
        if (p_stats != null)
            p_stats.EquipWeapon(newWeapon);

        var pooler = ServiceLocator.Get<ObjectPooler>();
        if (pooler != null)
            pooler.ConfigurePoolsForWeapons(new List<SOWeapon> { newWeapon });

        if (animator != null && newWeapon.animator != null)
            animator.runtimeAnimatorController = newWeapon.animator;

    }

    public void SetUpperBodyActive(bool active)
    {
        if (animator == null || _upperBodyLayerIndex < 0) return;
        float targetWeight = active ? 1f : 0f;
        animator.SetLayerWeight(_upperBodyLayerIndex, targetWeight);
    }


}
