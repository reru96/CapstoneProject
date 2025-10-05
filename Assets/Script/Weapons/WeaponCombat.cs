using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponCombat : MonoBehaviour
{
    [Header("Weapon Data")]
    public SOWeapon data;
    private PlayerStateMachine player;
    private bool isAttacking;

   
    public void Initialize(PlayerStateMachine owner)
    {
        player = owner;
    }

    public void HandleAttackStart(int number)
    {
        if (isAttacking) return;
        StartCoroutine(AttackRoutine(number));
    }

    public void HandleAttackEnd()
    { 
        isAttacking = false;
    }

    private IEnumerator AttackRoutine(int number)
    {
        isAttacking = true;

       
        yield return new WaitForSeconds(data.hitDelay);

        GameObject attackObj = Instantiate(
          data.attackType[number],
          player.transform.position,
          player.transform.rotation
      );

        if (data.swingSound)
            AudioSource.PlayClipAtPoint(data.swingSound, player.transform.position);

        yield return new WaitForSeconds(data.attackDuration);

        isAttacking = false;
    }
}
