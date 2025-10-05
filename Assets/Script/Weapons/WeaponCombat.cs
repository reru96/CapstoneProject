using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCombat : MonoBehaviour
{
    public SOWeapon data;
    private Collider hitbox;
    private bool isAttacking;
    private PlayerStateMachine player;

    private void Awake()
    {
        hitbox = GetComponent<Collider>();
    }

    public void Initialize(PlayerStateMachine owner)
    {
        player = owner;
        hitbox = GetComponent<Collider>();
        hitbox.enabled = false;
    }

    public void HandleAttackStart()
    {
        if (isAttacking) return;
        StartCoroutine(AttackRoutine());
    }

    public void HandleAttackEnd()
    {
        hitbox.enabled = false;
        isAttacking = false;
    }

    private IEnumerator AttackRoutine()
    {
        isAttacking = true;

       
        yield return new WaitForSeconds(data.hitDelay);

        hitbox.enabled = true;

        if (data.swingSound)
            AudioSource.PlayClipAtPoint(data.swingSound, transform.position);

        yield return new WaitForSeconds(data.attackDuration);

        hitbox.enabled = false;
        isAttacking = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isAttacking) return;

        if (other.CompareTag("Enemy"))
        {
            var health = other.GetComponent<LifeController>();
            if (health != null)
                health.AddHp((int)-data.strengthBonus);
        }
    }

}
