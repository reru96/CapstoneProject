using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitDetection : MonoBehaviour
{
    [SerializeField] private Transform handTransform;
    [SerializeField] private float raycastDistance = 2f;
    [SerializeField] private LayerMask targetLayerMask;
    [SerializeField] private HitWeapon hitWeapon = HitWeapon.Sword;
    [SerializeField] private float baseValue = 10f;

    private RaycastHit[] hits = new RaycastHit[8];
    private HashSet<Transform> hitSet = new HashSet<Transform>();
    private float cleanupTimer = 0f;
    private const float cleanupInterval = 0.75f;

    private void Awake()
    {
        enabled = false; 
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

    private void OnDisable()
    {
        hitSet.Clear();
    }

    private void FixedUpdate()
    {
        if (handTransform == null) return;

        Debug.DrawRay(handTransform.position, handTransform.forward * raycastDistance, Color.red, 0.1f);
        int count = Physics.RaycastNonAlloc(handTransform.position, handTransform.forward, hits, raycastDistance, targetLayerMask);

        if (count == 0) return;

        for (int i = 0; i < count; i++)
        {
            Transform t = hits[i].transform;
            if (t == null) continue;
            if (hitSet.Contains(t)) continue;

            hitSet.Add(t);

            if (t.TryGetComponent<IDamagable>(out var dam))
            {
                dam.TakeDamage(transform, hitWeapon, baseValue);
                continue;
            }

            if (t.TryGetComponent<EnemyStateMachine>(out var enemy))
            {
                PlayerStateMachine player = GetComponentInParent<PlayerStateMachine>();
                if (player != null && player.p_stats != null && player.combat != null && player.combat.data != null)
                {
                    float dmg = DamageUtility.CalculateDamage(player.p_stats, player.combat.data, enemy.enemyData);
                    DamageUtility.ApplyDamageToEnemy(enemy, dmg);
                    enemy.OnHit(transform.position);
                }
            }
        }
    }

    public void Activate() => enabled = true;
    public void Deactivate() => enabled = false;
}