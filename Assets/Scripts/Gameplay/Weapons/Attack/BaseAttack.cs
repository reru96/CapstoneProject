using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static Codice.Client.Common.EventTracking.TrackFeatureUseEvent.Features.DesktopGUI.Filters;

public class BaseAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float distance = 3f;
    public float duration = 0.3f;
    public float destroyDelay = 0.1f;
    public float repulseForce = 1f;

    protected Vector3 startPos;
    protected Vector3 endPos;
    protected float elapsed;

    protected PlayerStats playerStats;

    public void Initialize(PlayerStats stats)
    {
        playerStats = stats;
    }

    protected virtual void Start()
    {
        startPos = transform.position;
        endPos = startPos + transform.forward * distance;
    }

    protected virtual void Update()
    {
        if (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;

            if (elapsed >= duration)
                Destroy(gameObject, destroyDelay);
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy")) return;

        EnemyStateMachine enemy = other.GetComponent<EnemyStateMachine>();
        if (enemy == null)
        {
            Debug.LogWarning("Collider ha tag Enemy ma manca EnemyStateMachine!");
            return;
        }
        enemy.GetComponent<EnemyStateMachine>()?.OnHit(transform.position);
        DamageCalculation(enemy);

    }

    protected virtual void DamageCalculation(EnemyStateMachine enemy)
    {
        if (playerStats == null)
        {
            Debug.LogWarning("BaseAttack: playerStats non assegnato!");
            return;
        }

        if (playerStats.currentWeapon == null)
        {
            Debug.LogWarning("BaseAttack: il player non ha arma equipaggiata!");
            return;
        }

        if (enemy == null || enemy.enemyData == null)
        {
            Debug.LogWarning("BaseAttack: enemy o enemyData non assegnato!");
            return;
        }

        SOWeapon w = playerStats.currentWeapon;

  
        float strScale = Scaling.GetScalingMultiplier(w.strengthScaling);
        float dexScale = Scaling.GetScalingMultiplier(w.dexterityScaling);
        float intScale = Scaling.GetScalingMultiplier(w.intelligenceScaling);
        float faiScale = Scaling.GetScalingMultiplier(w.faithScaling);
        float arcScale = Scaling.GetScalingMultiplier(w.arcaneScaling);

        float physical = w.physicalBaseDamage + playerStats.Strength * strScale + playerStats.Dexterity * dexScale;
        float fire = w.fireBaseDamage + (w.scalesWithFaith ? playerStats.Faith * faiScale : 0);
        float magic = w.magicBaseDamage + (w.scalesWithIntelligence ? playerStats.Intelligence * intScale : 0);
        float lightning = w.lightningBaseDamage + (w.scalesWithFaith ? playerStats.Faith * faiScale : 0);
        float holy = w.holyBaseDamage + (w.scalesWithFaith ? playerStats.Faith * faiScale : 0);

        float finalPhysical = physical * (1 - (enemy.enemyData.physicalDefense / (enemy.enemyData.physicalDefense + 100)));
        float finalFire = fire * (1 - (enemy.enemyData.fireDefense / (enemy.enemyData.fireDefense + 100)));
        float finalMagic = magic * (1 - (enemy.enemyData.magicDefense / (enemy.enemyData.magicDefense + 100)));
        float finalLightning = lightning * (1 - (enemy.enemyData.lightningDefense / (enemy.enemyData.lightningDefense + 100)));
        float finalHoly = holy * (1 - (enemy.enemyData.holyDefense / (enemy.enemyData.holyDefense + 100)));

        float totalDamage = finalPhysical + finalFire + finalMagic + finalLightning + finalHoly;

        if (enemy.enemyData.isBackstabbed) totalDamage *= 1.5f;
        if (enemy.enemyData.isParried) totalDamage *= 2f;

        totalDamage *= Random.Range(0.9f, 1.1f);
        totalDamage = Mathf.Max(1f, totalDamage);

  
        LifeController life = enemy.GetComponent<LifeController>();
        if (life != null)
            life.AddHp(-(int)totalDamage);

     
        ShowDamagePopup(enemy, totalDamage);

        Debug.Log($"Total Damage Dealt: {totalDamage:F1}");
    }

    protected void ShowDamagePopup(EnemyStateMachine enemy, float damage)
    {
        if (enemy.enemyData.damagePopUpPrefab == null) return;

        Vector3 popupPos = enemy.transform.position + Vector3.up * 2f;
        popupPos += new Vector3(Random.Range(-0.3f, 0.3f), 0, Random.Range(-0.3f, 0.3f));

        GameObject popup = Instantiate(enemy.enemyData.damagePopUpPrefab, popupPos, Quaternion.identity);
        DamagePopUp dmgPopup = popup.GetComponent<DamagePopUp>();
        if (dmgPopup != null)
            dmgPopup.Setup(damage);
    }
}
