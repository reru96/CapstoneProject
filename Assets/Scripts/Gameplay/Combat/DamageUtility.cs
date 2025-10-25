using System.Collections;
using System.Collections.Generic;
using Codice.CM.Client.Differences.Graphic;
using UnityEngine;
using Core;
using Gameplay;

public static class DamageUtility 
{
    public static float CalculateDamage(PlayerStats pStats, SOWeapon w, SOEnemy enemyData)
    {
        if (pStats == null || w == null || enemyData == null)
        {
            Debug.LogWarning("DamageUtility: missing data");
            return 0f;
        }

        float strScale = Scaling.GetScalingMultiplier(w.strengthScaling);
        float dexScale = Scaling.GetScalingMultiplier(w.dexterityScaling);
        float intScale = Scaling.GetScalingMultiplier(w.intelligenceScaling);
        float faiScale = Scaling.GetScalingMultiplier(w.faithScaling);
        float arcScale = Scaling.GetScalingMultiplier(w.arcaneScaling);

        float physical = w.physicalBaseDamage + pStats.Strength * strScale + pStats.Dexterity * dexScale;
        float fire = w.fireBaseDamage + (w.scalesWithArcane ? pStats.Arcane * arcScale : 0f);
        float ice = w.iceBaseDamage + (w.scalesWithIntelligence ? pStats.Intelligence * intScale : 0f);
        float lightning = w.electricityBaseDamage + (w.scalesWithFaith ? pStats.Faith * faiScale : 0f);
        float piercing = w.piercingBaseDamage + (w.scalesWithDex ? pStats.Dexterity * dexScale : 0f);
        float slashing = w.slashingBaseDamage + (w.scalesWithStrenght ? pStats.Strength * strScale : 0f);

        float finalPhysical = physical * (1f - (enemyData.physicalDefense / (enemyData.physicalDefense + 100f)));
        float finalFire = fire * (1f - (enemyData.fireDefense / (enemyData.fireDefense + 100f)));
        float finalIce = ice * (1f - (enemyData.magicDefense / (enemyData.magicDefense + 100f)));
        float finalLightning = lightning * (1f - (enemyData.lightningDefense / (enemyData.lightningDefense + 100f)));
        float finalPiercing = piercing * (1f - (enemyData.piercingDefense / (enemyData.piercingDefense + 100f)));
        float finalSlashing = slashing * (1f - (enemyData.slashingDefense / (enemyData.slashingDefense + 100f)));

        float totalDamage = finalPhysical + finalFire + finalIce + finalLightning + finalPiercing + finalSlashing;
        totalDamage *= Random.Range(0.9f, 1.1f);
        totalDamage = Mathf.Max(1f, totalDamage);

        return totalDamage;
    }

    public static void ApplyDamageToEnemy(EnemyStateMachine enemy, float damage)
    {
        if (enemy == null || enemy.enemyData == null) return;

        LifeController life = enemy.GetComponent<LifeController>();
        if (life != null)
            life.AddHp(-(int)damage);

        if (enemy.enemyData.damagePopUpPrefab != null)
        {
            Vector3 popupPos = enemy.transform.position + Vector3.up * 2f;
            popupPos += new Vector3(Random.Range(-0.3f, 0.3f), 0f, Random.Range(-0.3f, 0.3f));
            GameObject popup = Object.Instantiate(enemy.enemyData.damagePopUpPrefab, popupPos, Quaternion.identity);
            var dmgPopup = popup.GetComponent<DamagePopUp>();
            if (dmgPopup != null) dmgPopup.Setup(damage);
        }
    }
}
