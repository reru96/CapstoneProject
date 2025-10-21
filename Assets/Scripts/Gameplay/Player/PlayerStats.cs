using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{
   
    public SOPlayerClass playerClass;
    public SOWeapon currentWeapon;
    public SOArmor currentArmor;

    public float buffStrength;
    public float buffDexterity;
    public float buffIntelligence;
    public float buffFaith;
    public float buffArcane;

    public GameObject hand;

    public float Strength { get; private set; }
    public float Dexterity { get; private set; }
    public float Intelligence { get; private set; }
    public float Faith { get; private set; }
    public float Arcane { get; private set; }

    public delegate void OnStatsRecalculated();
    public event OnStatsRecalculated StatsUpdated;

    private void Start()
    {
        RecalculateStats();
    }

    public void RecalculateStats()
    {
        Strength = playerClass.baseStrength;
        Dexterity = playerClass.baseDexterity;
        Intelligence = playerClass.baseIntelligence;
        Faith = playerClass.baseFaith;
        Arcane = playerClass.baseArcane;

        if (currentWeapon != null)
        {
            Strength += currentWeapon.strengthScaling != ScalingGrade.None ? currentWeapon.strengthBonus : 0;
            Dexterity += currentWeapon.dexterityScaling != ScalingGrade.None ? currentWeapon.dexterityBonus : 0;
            Intelligence += currentWeapon.intelligenceBonus;
            Faith += currentWeapon.faithScaling != ScalingGrade.None ? 0.5f : 0;
            Arcane += currentWeapon.arcaneScaling != ScalingGrade.None ? 0.3f : 0;
        }


        if (currentArmor != null)
        {
            Strength += currentArmor.strengthBonus;
            Dexterity += currentArmor.dexterityBonus;
            Intelligence += currentArmor.intelligenceBonus;
            Faith += currentArmor.faithBonus;
            Arcane += currentArmor.arcaneBonus;
        }

        Strength += buffStrength;
        Dexterity += buffDexterity;
        Intelligence += buffIntelligence;
        Faith += buffFaith;
        Arcane += buffArcane;

        StatsUpdated?.Invoke();
    }

    public void EquipWeapon(SOWeapon newWeapon)
    {
        currentWeapon = newWeapon;
        GameObject newWeaponObj = Instantiate(newWeapon.prefab, hand.transform);
        newWeaponObj.transform.localPosition = Vector3.zero;
        newWeaponObj.transform.localRotation = Quaternion.identity;
        RecalculateStats();
    }

    public void ApplyBuff(string stat, float value, float duration)
    {
        StartCoroutine(ApplyBuffCoroutine(stat, value, duration));
    }

    private IEnumerator ApplyBuffCoroutine(string stat, float value, float duration)
    {
        switch (stat)
        {
            case "Strength": buffStrength += value; break;
            case "Dexterity": buffDexterity += value; break;
            case "Intelligence": buffIntelligence += value; break;
            case "Faith": buffFaith += value; break;
            case "Arcane": buffArcane += value; break;
        }

        RecalculateStats();
        yield return new WaitForSeconds(duration);

        switch (stat)
        {
            case "Strength": buffStrength -= value; break;
            case "Dexterity": buffDexterity -= value; break;
            case "Intelligence": buffIntelligence -= value; break;
            case "Faith": buffFaith -= value; break;
            case "Arcane": buffArcane -= value; break;
        }

        RecalculateStats();
    }
}
