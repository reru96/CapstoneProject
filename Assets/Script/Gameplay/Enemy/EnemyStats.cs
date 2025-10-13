using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    public int maxHP = 100;
    public int currentHP;

    [Header("Defenses")]
    public float physicalDefense = 40f;
    public float fireDefense = 20f;
    public float magicDefense = 25f;
    public float lightningDefense = 15f;
    public float holyDefense = 30f;

    [Header("State Flags")]
    public bool isBackstabbed;
    public bool isParried;

    private void Awake()
    {
        currentHP = maxHP;
    }

    public void TakeDamage(float amount)
    {
        currentHP -= Mathf.RoundToInt(amount);
        if (currentHP <= 0)
            Die();
    }

    private void Die()
    {
        Debug.Log($"{name} è morto!");
    }
}
