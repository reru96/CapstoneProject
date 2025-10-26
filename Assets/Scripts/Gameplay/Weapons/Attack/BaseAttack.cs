using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

public class BaseAttack : MonoBehaviour
{
    [Header("Attack Settings")]
    public float distance = 3f;
    public float duration = 0.3f;
    public float destroyDelay = 0.1f;
    public float repulseForce = 1f;

    [SerializeField]protected Vector3 startPos;
    [SerializeField]protected Vector3 endPos;
    protected float elapsed;
    [SerializeField]protected DamageType AttackType;
    protected PlayerStats playerStats;

    private Poolable poolable;

    public void Initialize(PlayerStats stats)
    {
        playerStats = stats;
    }

    protected virtual void OnEnable()
    {
        startPos = transform.position;
        endPos = startPos + transform.forward * distance;
        elapsed = 0f;

        poolable = GetComponent<Poolable>();
        if (poolable != null)
            poolable.SetReturnDelay(destroyDelay);
    }

    protected virtual void Update()
    {
        if (elapsed < duration)
        {
            transform.position = Vector3.Lerp(startPos, endPos, elapsed / duration);
            elapsed += Time.deltaTime;
        }
    }

    //protected virtual void OnTriggerEnter(Collider other)
    //{
    //    if (!other.CompareTag("Enemy")) return;

    //    var enemy = other.GetComponent<EnemyStateMachine>();
    //    if (enemy == null)
    //    {
    //        return;
    //    }

    //    enemy.OnHit(transform.position);
    //    DamageUtility.DamageCalculation();

    //    poolable?.ReturnToPool();
    //}

    //protected virtual void DamageCalculation(EnemyStateMachine enemy)
    //{
    //    if (playerStats == null)
    //    {
    //        Debug.LogWarning("BaseAttack: playerStats non assegnato!");
    //        return;
    //    }

    //    if (playerStats.currentWeapon == null)
    //    {
    //        Debug.LogWarning("BaseAttack: il player non ha arma equipaggiata!");
    //        return;
    //    }

    //    if (enemy == null || enemy.enemyData == null)
    //    {
    //        Debug.LogWarning("BaseAttack: enemy o enemyData non assegnato!");
    //        return;
    //    }

    //    SOWeapon w = playerStats.currentWeapon;

  
    //    float strScale = Scaling.GetScalingMultiplier(w.strengthScaling);
    //    float dexScale = Scaling.GetScalingMultiplier(w.dexterityScaling);
    //    float intScale = Scaling.GetScalingMultiplier(w.intelligenceScaling);
    //    float faiScale = Scaling.GetScalingMultiplier(w.faithScaling);
    //    float arcScale = Scaling.GetScalingMultiplier(w.arcaneScaling);

    //    float physical = w.physicalBaseDamage + playerStats.Strength * strScale + playerStats.Dexterity * dexScale;
    //    float fire = w.fireBaseDamage + (w.scalesWithArcane ? playerStats.Arcane * arcScale : 0);
    //    float ice = w.iceBaseDamage + (w.scalesWithIntelligence ? playerStats.Intelligence * intScale : 0);
    //    float lightning = w.electricityBaseDamage + (w.scalesWithFaith ? playerStats.Faith * faiScale : 0);
    //    float piercing = w.piercingBaseDamage + (w.scalesWithDex ? playerStats.Dexterity * dexScale : 0);
    //    float slashing = w.slashingBaseDamage + (w.scalesWithStrenght ? playerStats.Strength * strScale : 0);

    //    float finalPhysical = physical * (1 - (enemy.enemyData.physicalDefense / (enemy.enemyData.physicalDefense + 100)));
    //    float finalFire = fire * (1 - (enemy.enemyData.fireDefense / (enemy.enemyData.fireDefense + 100)));
    //    float finalIce = ice * (1 - (enemy.enemyData.magicDefense / (enemy.enemyData.magicDefense + 100)));
    //    float finalLightning = lightning * (1 - (enemy.enemyData.lightningDefense / (enemy.enemyData.lightningDefense + 100)));
    //    float finalPiercing = piercing * (1 - (enemy.enemyData.piercingDefense / (enemy.enemyData.piercingDefense + 100)));
    //    float finalSlashing = slashing * (1- (enemy.enemyData.slashingDefense /(enemy.enemyData.slashingDefense + 100)));

    //    float totalDamage = finalPhysical + finalFire + finalIce + finalLightning;

    //    totalDamage *= Random.Range(0.9f, 1.1f);
    //    totalDamage = Mathf.Max(1f, totalDamage);

  
    //    LifeController life = enemy.GetComponent<LifeController>();
    //    if (life != null)
    //        life.AddHp(-(int)totalDamage);

     
    //    ShowDamagePopup(enemy, totalDamage);

    //    Debug.Log($"Total Damage Dealt: {totalDamage:F1}");
    //}

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
