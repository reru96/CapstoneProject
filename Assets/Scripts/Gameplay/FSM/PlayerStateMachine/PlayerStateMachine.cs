using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using Core;
using Gameplay;
using UnityEngine.AI;

public class PlayerStateMachine : StateMachine
{
    [Header("Data")]
    public SOPlayerClass p_data;
    public SOWeapon weapon;

    [Header("Components")]
    public PlayerStats p_stats;
    public Animator animator { get; private set; }
    public Rigidbody rb { get; private set; }
    public NavMeshAgent agent { get; private set; }

    [HideInInspector] public WeaponCombat weaponInstance;
    private InventoryManager _inventory;

    [Header("Movement")]
    [HideInInspector] public Vector3 currentVelocity = Vector3.zero;
    public float accelerationTime = 0.2f;
    public float rotationSpeed = 25f;

    [Header("State Flags")]
    public bool isInvincible = false;
    public bool isMoving = false;
    public bool isDodging = false;
    public bool isAttacking = false;

    [Header("Animator Layers")]
    [SerializeField] private string upperBodyLayerName = "UpperBody";
    private int _upperBodyLayerIndex = -1;
    public int UpperBodyLayerIndex => _upperBodyLayerIndex;

    private void Awake()
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
                Debug.LogWarning($"[PlayerStateMachine] Layer '{upperBodyLayerName}' non trovato nell'Animator.");
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

            if (weaponInstance != null)
                weaponInstance.data = weapon;

            var pooler = ServiceLocator.Get<ObjectPooler>();
            if (pooler != null)
            {
                pooler.ClearAllPools();
                pooler.ConfigurePoolsForWeapons(new List<SOWeapon> { weapon });
            }

            if (animator != null && weapon.animator != null)
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

        if (weaponInstance != null)
            weaponInstance.data = newWeapon;

        var pooler = ServiceLocator.Get<ObjectPooler>();
        if (pooler != null)
            pooler.ConfigurePoolsForWeapons(new List<SOWeapon> { newWeapon });

        if (animator != null && newWeapon.animator != null)
            animator.runtimeAnimatorController = newWeapon.animator;

        Debug.Log($"[PlayerStateMachine] Equipped new weapon: {newWeapon.name}");
    }
    public void SetUpperBodyActive(bool active)
    {
        if (animator == null || _upperBodyLayerIndex < 0) return;
        animator.SetLayerWeight(_upperBodyLayerIndex, active ? 1f : 0f);
    }

    private void OnEnable()
    {
        GameEvent.OnPlayerDead += EnterDeadState;
    }

    private void OnDisable()
    {
        GameEvent.OnPlayerDead -= EnterDeadState;
    }

    private void EnterDeadState()
    {
        Debug.Log("[PlayerStateMachine] PlayerDead evento ricevuto → entro in PlayerDeadState");
        SwitchState(new PlayerDeadState(this));
    }
}
