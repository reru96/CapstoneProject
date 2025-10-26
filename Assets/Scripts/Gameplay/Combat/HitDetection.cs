using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    [SerializeField] private Collider triggerCollider;
    [SerializeField] private float baseValue = 10f;
    [SerializeField] private LayerMask targetLayerMask = ~0;
    [SerializeField] private HitWeapon hitWeapon = HitWeapon.Sword;

    private HashSet<Transform> hitSet = new HashSet<Transform>();
    private float cleanupTimer = 0f;
    private const float cleanupInterval = 0.75f;

    private WeaponCombat ownerWeapon;
    private PlayerStateMachine ownerPlayer;
    private PlayerStats playerStats;
    private Collider[] selfColliders;

    private void Awake()
    {
        selfColliders = GetComponentsInChildren<Collider>(true);
        if (triggerCollider != null)
            triggerCollider.isTrigger = true;
    }

    private void OnEnable()
    {
        hitSet.Clear();
        cleanupTimer = 0f;
        if (triggerCollider != null)
            triggerCollider.enabled = true;
    }

    private void OnDisable()
    {
        hitSet.Clear();
        if (triggerCollider != null)
            triggerCollider.enabled = false;
    }

    private void Update()
    {
        cleanupTimer += Time.deltaTime;
        if (cleanupTimer >= cleanupInterval)
        {
            hitSet.Clear();
            cleanupTimer = 0f;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & targetLayerMask) == 0) return;
        if (IsSelfCollider(other)) return;

        Transform t = other.transform;
        if (hitSet.Contains(t)) return;

        hitSet.Add(t);

        if (other.TryGetComponent<IDamagable>(out var dam))
        {
            dam.TakeDamage(transform, hitWeapon, baseValue);
            return;
        }

        if (other.TryGetComponent<EnemyStateMachine>(out var enemy))
        {
            float dmg = (ownerPlayer != null && ownerWeapon != null && ownerWeapon.data != null)
                ? DamageUtility.CalculateDamage(ownerPlayer.p_stats, ownerWeapon.data, enemy.enemyData)
                : baseValue;

            DamageUtility.ApplyDamageToEnemy(enemy, dmg);
            enemy.OnHit(transform.position);
        }
    }

    private bool IsSelfCollider(Collider c)
    {
        if (selfColliders == null) return false;
        foreach (var col in selfColliders)
        {
            if (col == c) return true;
        }
        return false;
    }
    public Collider GetTriggerCollider() => triggerCollider;
    public void SetBaseValue(float v) => baseValue = v;
    public void SetLayerMask(LayerMask mask) => targetLayerMask = mask;
    public void SetOwner(WeaponCombat weapon, PlayerStateMachine player)
    {
        ownerWeapon = weapon;
        ownerPlayer = player;
    }
}