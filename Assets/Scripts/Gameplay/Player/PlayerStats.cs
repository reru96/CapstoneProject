using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static PlasticGui.PlasticTableCell;

public class PlayerStats : MonoBehaviour
{
    private const float levelMultiplier = 1.2f;
    public SOPlayerClass playerClass;
    public SOWeapon currentWeapon;
    public SOArmor currentArmor; 
    public GameObject hand;

    public float buffHealth;
    public float buffMana;
    public float buffStamina;
    public float buffStrength;
    public float buffDexterity;
    public float buffIntelligence;
    public float buffFaith;
    public float buffArcane;

    public int exp;
    public int expToNextLevel = 100;    
    public int Level { get; private set; } = 1;  
    public int StatPoints { get; private set; } = 0;
    public float Health {  get; private set; }
    public float Mana { get; private set; }
    public float Stamina{get; private set;}
    public float Strength { get; private set; }
    public float Dexterity { get; private set; }
    public float Intelligence { get; private set; }
    public float Faith { get; private set; }
    public float Arcane { get; private set; }

    public Dictionary<DamageType, float> Defenses { get; private set; } = new Dictionary<DamageType, float>();

    public delegate void OnStatsRecalculated();
    public event OnStatsRecalculated StatsUpdated;

    private void Start()
    {
        RecalculateStats();
    }

    public void RecalculateStats()
    {
        Health = playerClass.health;
        Mana = playerClass.mana;
        Stamina = playerClass.stamina;
        Strength = playerClass.baseStrength;
        Dexterity = playerClass.baseDexterity;
        Intelligence = playerClass.baseIntelligence;
        Faith = playerClass.baseFaith;
        Arcane = playerClass.baseArcane;

        if (currentWeapon != null)
        {
            Strength += currentWeapon.strengthScaling != ScalingGrade.None ? currentWeapon.strengthBonus : 0;
            Dexterity += currentWeapon.dexterityScaling != ScalingGrade.None ? currentWeapon.dexterityBonus : 0;
            Intelligence += currentWeapon.intelligenceScaling != ScalingGrade.None? currentWeapon.intelligenceBonus : 0;
            Faith += currentWeapon.faithScaling != ScalingGrade.None ? currentWeapon.faithBonus : 0;
            Arcane += currentWeapon.arcaneScaling != ScalingGrade.None ? currentWeapon.arcaneBonus : 0;
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

        Defenses[DamageType.Force] = Level * 2f;
        Defenses[DamageType.Slashing] = Strength;
        Defenses[DamageType.Piercing] = Dexterity;
        Defenses[DamageType.Ice] = Intelligence;
        Defenses[DamageType.Electricity] = Faith;
        Defenses[DamageType.Fire] = Arcane;

        StatsUpdated?.Invoke();
    }

    public void EquipWeapon(SOWeapon newWeapon)
    {
        currentWeapon = newWeapon;
       
        foreach (Transform child in hand.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
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

    public void AddExperience(int amount)
    {
        exp += amount;
        if (exp >= expToNextLevel)
        {
            LevelUp();
        }
    }

    private void LevelUp()
    {
        exp -= expToNextLevel;
        Level++;
        StatPoints += 5; 
        expToNextLevel = Mathf.RoundToInt(expToNextLevel * levelMultiplier);

        RecalculateStats();
    }

    public bool AllocateStatPoint(string stat)
    {
        if (StatPoints <= 0)
            return false;

        switch (stat)
        {
            case "Strength": playerClass.baseStrength++; break;
            case "Dexterity": playerClass.baseDexterity++; break;
            case "Intelligence": playerClass.baseIntelligence++; break;
            case "Faith": playerClass.baseFaith++; break;
            case "Arcane": playerClass.baseArcane++; break;
            case "Health": playerClass.health += 10; break;
            case "Mana": playerClass.mana += 5; break;
            case "Stamina": playerClass.stamina += 5; break;
            default:
                Debug.LogWarning($"Stat '{stat}' non riconosciuta.");
                return false;
        }

        StatPoints--;
        RecalculateStats();
        return true;
    }

    public float GetDamageAfterDefense(float incomingDamage, DamageType type)
    {
        if (Defenses.TryGetValue(type, out float defense))
        {
            incomingDamage -= defense; 
            if (incomingDamage < 0) incomingDamage = 0;
        }
        return incomingDamage;
    }
}
