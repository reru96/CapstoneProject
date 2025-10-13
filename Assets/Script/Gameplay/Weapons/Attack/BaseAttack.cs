using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseAttack : MonoBehaviour
{
    [SerializeField] protected float distance;
    [SerializeField] protected float duration;
    [SerializeField] protected float destroyDelay;
    [SerializeField] protected float repulseForce;

    [SerializeField] protected Vector3 startPos;
    [SerializeField] protected Vector3 endPos;
    [SerializeField] protected Vector3 dir;
    [SerializeField] protected float elapsed;
    public float damage;

    protected PlayerStats playerStats;

    protected virtual void Start()
    {
        playerStats = GetComponent<PlayerStats>();
        startPos = transform.position;
        endPos = startPos + dir * distance;
    }

    protected virtual void Update()
    {
        if (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;

            if (elapsed >= duration)
            {
                transform.position = endPos;
                Destroy(gameObject, destroyDelay);
            }
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Enemy"))
        {
            EnemyStats enemy = other.GetComponent<EnemyStats>();
            if (enemy != null)
            {
                DamageCalculation(enemy);
                enemy.GetComponent<EnemyStateMachine>()?.OnHit(transform.position);
                enemy.transform.position += transform.forward * repulseForce;
            }
        }
    }

    protected virtual void DamageCalculation(EnemyStats enemy)
    {
        SOWeapon w = playerStats.currentWeapon;

        float strScale = Scaling.GetScalingMultiplier(w.strengthScaling);
        float dexScale = Scaling.GetScalingMultiplier(w.dexterityScaling);
        float intScale = Scaling.GetScalingMultiplier(w.intelligenceScaling);
        float faiScale = Scaling.GetScalingMultiplier(w.faithScaling);
        float arcScale = Scaling.GetScalingMultiplier(w.arcaneScaling);

        float physical = w.physicalBaseDamage + (playerStats.Strength * strScale) + (playerStats.Dexterity * dexScale);
        float fire = w.fireBaseDamage + (w.scalesWithFaith ? playerStats.Faith * faiScale : 0);
        float magic = w.magicBaseDamage + (w.scalesWithIntelligence ? playerStats.Intelligence * intScale : 0);
        float lightning = w.lightningBaseDamage + (w.scalesWithFaith ? playerStats.Faith * faiScale : 0);
        float holy = w.holyBaseDamage + (w.scalesWithFaith ? playerStats.Faith * faiScale : 0);

        float finalPhysical = physical * (1 - (enemy.physicalDefense / (enemy.physicalDefense + 100)));
        float finalFire = fire * (1 - (enemy.fireDefense / (enemy.fireDefense + 100)));
        float finalMagic = magic * (1 - (enemy.magicDefense / (enemy.magicDefense + 100)));
        float finalLightning = lightning * (1 - (enemy.lightningDefense / (enemy.lightningDefense + 100)));
        float finalHoly = holy * (1 - (enemy.holyDefense / (enemy.holyDefense + 100)));

        float totalDamage = finalPhysical + finalFire + finalMagic + finalLightning + finalHoly;

        if (enemy.isBackstabbed) totalDamage *= 1.5f;
        if (enemy.isParried) totalDamage *= 2f;

        totalDamage *= UnityEngine.Random.Range(0.9f, 1.1f);
        totalDamage = Mathf.Max(1, totalDamage);

        enemy.TakeDamage(totalDamage);
        Debug.Log($"Total Damage Dealt: {totalDamage:F1}");
    }
}
