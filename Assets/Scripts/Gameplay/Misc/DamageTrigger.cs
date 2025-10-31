using System.Collections;
using System.Collections.Generic;
using Core;
using GamePlay;
using UnityEngine;

public class DamageTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    { 
        var shooterStats = GetComponentInParent<PlayerStateMachine>();
        var weaponData = GetComponentInParent<WeaponCombat>();
        if (!other.CompareTag("Enemy")) return;

        if (other.TryGetComponent<EnemyStateMachine>(out var enemy))
        {
            enemy.OnHit(transform.position);
            float dmg = DamageUtility.CalculateDamage(shooterStats.p_stats, weaponData.data, enemy.enemyData);
            DamageUtility.ApplyDamageToEnemy(enemy, dmg);
        }
    }
}
