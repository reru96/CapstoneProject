using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    [SerializeField] private Transform handTransform;
    [SerializeField] private float raycastDistance = 2f;
    [SerializeField] private float raycastRadius = 0.25f;
    [SerializeField] private LayerMask targetLayerMask = ~0;
    [SerializeField] private HitWeapon hitWeapon = HitWeapon.Sword;
    [SerializeField] private float baseValue = 10f;

    private RaycastHit[] hits = new RaycastHit[16];
    private HashSet<Transform> hitSet = new HashSet<Transform>();
    private float cleanupTimer = 0f;
    private const float cleanupInterval = 0.75f;

    private WeaponCombat ownerWeapon;
    private PlayerStateMachine ownerPlayer;
    private PlayerStats playerStats;
    private Collider[] selfColliders;

    private void Awake()
    {
        enabled = false;
        selfColliders = GetComponentsInChildren<Collider>(true);
    }

    private void OnEnable()
    {
        hitSet.Clear();
        cleanupTimer = 0f;
    }

    private void OnDisable()
    {
        hitSet.Clear();
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

    private void FixedUpdate()
    {
        if (handTransform == null)
        {
            if (ownerWeapon != null && ownerWeapon.transform != null)
                handTransform = ownerWeapon.transform;
            if (ownerPlayer != null && ownerPlayer.transform != null)
            {
                Transform playerHand = ownerPlayer.transform.Find("Hand");
                if (playerHand != null) handTransform = playerHand;
            }
            if (handTransform == null) return;
        }

        Vector3 origin = handTransform.position;
        Vector3 dir = handTransform.forward;

        Debug.DrawRay(origin, dir * raycastDistance, Color.red, 0.1f);

        int count = Physics.SphereCastNonAlloc(origin, raycastRadius, dir, hits, raycastDistance, targetLayerMask, QueryTriggerInteraction.Ignore);
        if (count == 0) return;

        for (int i = 0; i < count; i++)
        {
            var hit = hits[i];
            Transform t = hit.transform;
            if (t == null) continue;
            if (hitSet.Contains(t)) continue;
            if (IsSelfCollider(hit.collider)) continue;

            hitSet.Add(t);

            if (t.TryGetComponent<IDamagable>(out var dam))
            {
                dam.TakeDamage(transform, hitWeapon, baseValue);
                continue;
            }

            if (t.TryGetComponent<EnemyStateMachine>(out var enemy))
            {
                if (ownerPlayer != null && ownerPlayer.p_stats != null && ownerWeapon != null && ownerWeapon.data != null)
                {
                    float dmg = DamageUtility.CalculateDamage(ownerPlayer.p_stats, ownerWeapon.data, enemy.enemyData);
                    DamageUtility.ApplyDamageToEnemy(enemy, dmg);
                    enemy.OnHit(origin);
                }
                else
                {
                    float dmg = baseValue;
                    DamageUtility.ApplyDamageToEnemy(enemy, dmg);
                    enemy.OnHit(origin);
                }
            }
        }
    }

    private bool IsSelfCollider(Collider c)
    {
        if (selfColliders == null) return false;
        for (int i = 0; i < selfColliders.Length; i++)
        {
            if (selfColliders[i] == c) return true;
        }
        return false;
    }

    public void Activate()
    {
        hitSet.Clear();
        enabled = true;
    }

    public void Deactivate()
    {
        enabled = false;
    }

    public void SetHandTransform(Transform t) => handTransform = t;
    public void SetBaseValue(float v) => baseValue = v;
    public void SetLayerMask(LayerMask mask) => targetLayerMask = mask;
}