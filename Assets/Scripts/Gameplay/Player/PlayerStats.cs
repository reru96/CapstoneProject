using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


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
    public float Health { get; private set; }
    public float Mana { get; private set; }
    public float Stamina { get; private set; }
    public float Strength { get; private set; }
    public float Dexterity { get; private set; }
    public float Intelligence { get; private set; }
    public float Faith { get; private set; }
    public float Arcane { get; private set; }

    public Dictionary<DamageType, float> Defenses { get; private set; } = new Dictionary<DamageType, float>();

    public delegate void OnStatsRecalculated();
    public event OnStatsRecalculated StatsUpdated;
    public event Action<int> ExpChanged;
    public event Action<int> StatPointsChanged;

    private void Start()
    {
        RecalculateStats();
        ExpChanged?.Invoke(exp);
        StatPointsChanged?.Invoke(StatPoints);
    }

    public void RecalculateStats()
    {
        if (playerClass == null)
        {
            Debug.LogWarning("[PlayerStats] playerClass mancante");
            return;
        }

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
            Strength += currentWeapon.strengthBonus;
            Dexterity += currentWeapon.dexterityBonus;
            Intelligence += currentWeapon.intelligenceBonus;
            Faith += currentWeapon.faithBonus;
            Arcane += currentWeapon.arcaneBonus;
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
        if (newWeapon == null) return;

        currentWeapon = newWeapon;

        if (hand != null)
        {
            foreach (Transform child in hand.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            if (newWeapon.prefab != null)
            {
                GameObject newWeaponObj = Instantiate(newWeapon.prefab, hand.transform);
                newWeaponObj.transform.localPosition = Vector3.zero;
                newWeaponObj.transform.localRotation = Quaternion.identity;
            }
        }

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
        ExpChanged?.Invoke(exp);
    }

    public void ProcessLevelUpAtRestPoint()
    {
        bool leveledUp = false;
        while (exp >= expToNextLevel)
        {
            exp -= expToNextLevel;
            Level++;
            StatPoints += 5;
            expToNextLevel = Mathf.RoundToInt(expToNextLevel * levelMultiplier);
            leveledUp = true;
        }

        if (leveledUp)
        {
            ExpChanged?.Invoke(exp);
            StatPointsChanged?.Invoke(StatPoints);
            RecalculateStats();
        }
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
        StatPointsChanged?.Invoke(StatPoints);
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

    public void ResetStats()
    {
        if (playerClass == null)
        {
            Debug.LogWarning("[PlayerStats] playerClass mancante, impossibile resettare le statistiche");
            return;
        }

        Level = 1;
        exp = 0;
        expToNextLevel = 100;
        StatPoints = 0;

        Health = playerClass.health;
        Mana = playerClass.mana;
        Stamina = playerClass.stamina;
        Strength = playerClass.baseStrength;
        Dexterity = playerClass.baseDexterity;
        Intelligence = playerClass.baseIntelligence;
        Faith = playerClass.baseFaith;
        Arcane = playerClass.baseArcane;

        buffHealth = 0;
        buffMana = 0;
        buffStamina = 0;
        buffStrength = 0;
        buffDexterity = 0;
        buffIntelligence = 0;
        buffFaith = 0;
        buffArcane = 0;

        currentWeapon = null;
        currentArmor = null;

        if (hand != null)
        {
            foreach (Transform child in hand.transform)
                Destroy(child.gameObject);
        }

        RecalculateStats();
    }

}
