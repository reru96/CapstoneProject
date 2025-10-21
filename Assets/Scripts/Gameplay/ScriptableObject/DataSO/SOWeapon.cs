using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("RPG/Weapon"))]
public class SOWeapon : SORunItem
{
    public GameObject prefab;
    public GameObject[] attackType;
    public AudioClip swingSound;
    public float attackDuration;
    public float hitDelay = 0.3f;
    public RuntimeAnimatorController animator;

    [Header("Base Damage")]
    public float physicalBaseDamage = 50f;
    public float fireBaseDamage = 0f;
    public float magicBaseDamage = 0f;
    public float lightningBaseDamage = 0f;
    public float holyBaseDamage = 0f;

    [Header("Scaling Grades")]
    public ScalingGrade strengthScaling = ScalingGrade.D;
    public ScalingGrade dexterityScaling = ScalingGrade.D;
    public ScalingGrade intelligenceScaling = ScalingGrade.E;
    public ScalingGrade faithScaling = ScalingGrade.E;
    public ScalingGrade arcaneScaling = ScalingGrade.E;

    [Header("Elemental Scaling Influence")]
    public bool scalesWithIntelligence; 
    public bool scalesWithFaith;        
    public bool scalesWithArcane;

    public float strengthBonus;
    public float dexterityBonus;
    public float intelligenceBonus;
    public float defenseBonus;
    public float speedBonus;

}
