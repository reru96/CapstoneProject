using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = ("RPG/Weapon"))]
public class SOWeapon : SORunItem
{
    public GameObject prefab;
    public GameObject[] attackType;
    public string[] swingSound;
    public float attackDuration = 1f;
    public PanelType panelType;
    public HitWeapon hitType;
    public float hitDelay = 0.3f;
    public RuntimeAnimatorController animator;
    public ParticleSystem[] particleSystem;

    public bool isRanged = false;
    public GameObject projectilePrefab;
    public float projectileSpeed = 15f;
    public float attackWindow = 0.25f;

    public float baseDamage = 50f;
    public LayerMask hitLayerMask = ~0;
    public GameObject hitDetectionPrefab;

    public float physicalBaseDamage = 50f;
    public float fireBaseDamage = 0f;
    public float iceBaseDamage = 0f;
    public float electricityBaseDamage = 0f;
    public float slashingBaseDamage = 0f;
    public float piercingBaseDamage = 0f;

    public ScalingGrade strengthScaling = ScalingGrade.D;
    public ScalingGrade dexterityScaling = ScalingGrade.D;
    public ScalingGrade intelligenceScaling = ScalingGrade.E;
    public ScalingGrade faithScaling = ScalingGrade.E;
    public ScalingGrade arcaneScaling = ScalingGrade.E;

    public bool scalesWithIntelligence;
    public bool scalesWithFaith;
    public bool scalesWithArcane;
    public bool scalesWithDex;
    public bool scalesWithStrenght;

    public float strengthBonus;
    public float dexterityBonus;
    public float intelligenceBonus;
    public float faithBonus;
    public float arcaneBonus;
    public float defenseBonus;
    public float speedBonus;
}
