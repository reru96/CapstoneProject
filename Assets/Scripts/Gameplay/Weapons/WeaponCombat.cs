using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

public class WeaponCombat : MonoBehaviour
{
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

        var pooler = ServiceLocator.Get<ObjectPooler>();
        if (pooler == null)
        {
            Debug.LogError("[WeaponCombat] ObjectPooler non trovato!");
            yield break;
        }

        GameObject prefabGO = data.attackType[number];
        if (prefabGO == null)
        {
            Debug.LogWarning("[WeaponCombat] Prefab attacco non assegnato!");
            yield break;
        }

        BaseAttack attackObj = pooler.Spawn<BaseAttack>(prefabGO, player.transform.position, player.transform.rotation);


        if (attackObj != null)
        {
            attackObj.Initialize(player.p_stats);

            if (data.swingSound != null)
                AudioSource.PlayClipAtPoint(data.swingSound, player.transform.position);
        }
        else
        {
            Debug.LogWarning("[WeaponCombat] Spawn dal pool fallito per: " + prefabGO.name);
        }

        yield return new WaitForSeconds(data.attackDuration);
        isAttacking = false;
    }
}



