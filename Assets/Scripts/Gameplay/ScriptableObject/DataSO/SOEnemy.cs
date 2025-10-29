using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu(fileName =("Enemy"), menuName = ("Enemies/Enemy"))]
public class SOEnemy : ScriptableObject
{
    public string nameEnemy;
    public EnemyType enemyType;
    public int maxHP = 100;
    public int currentHP;
    public GameObject enemyPrefab;
    public float expDrop = 100f; 
    public int coinReward = 500;

    public float physicalDefense = 40f;
    public float fireDefense = 20f;
    public float magicDefense = 25f;
    public float lightningDefense = 15f;
    public float piercingDefense = 30f;
    public float slashingDefense = 30f;

    public GameObject damagePopUpPrefab;
    public float basePhysicalDamage = 0;
    public float baseFireDamage = 0;
    public float baseIceDamage = 0;
    public float baseLightningDamage = 0;
    public float basePiercingDamage= 0;
    public float baseSlashingDamage= 0;
}

public enum EnemyType
{
    CommonMelee,
    CommonRange,
    EliteMelee,
    EliteRange,
    Boss
}