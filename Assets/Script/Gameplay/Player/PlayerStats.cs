using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : MonoBehaviour
{

    public SOPlayerClass baseData;
    public SOWeapon equippedWeapon;

    public float Strenght { get; private set; }
    public float Intelligence { get; private set; }

    public float Dexterity { get; private set; }
    public float Defense { get; private set; }
    public float Speed { get; private set; }

    private void Start()
    {
        RecalculateStats();
    }

    public void EquipWeapon(SOWeapon newWeapon)
    {
        equippedWeapon = newWeapon;
        RecalculateStats();
    }

    public void UnequipWeapon()
    {
        equippedWeapon = null;
        RecalculateStats();
    }

    public void RecalculateStats()
    {
        Strenght = baseData.strenght;
        Intelligence = baseData.intelligence;
        Dexterity = baseData.dexterity;
        Defense = baseData.defense;
        Speed = baseData.speed;

        if (equippedWeapon != null)
        { 
            Strenght += equippedWeapon.strengthBonus;
            Intelligence += equippedWeapon.intelligenceBonus;
            Dexterity += equippedWeapon.dexterityBonus;
            Defense += equippedWeapon.defenseBonus;
            Speed += equippedWeapon.speedBonus;
        }
    }
}
