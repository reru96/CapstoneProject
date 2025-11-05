using System;
using System.Collections;
using Core;
using UnityEngine;

public class WeaponCombat : MonoBehaviour
{
    private PlayerStateMachine player;
    public SOWeapon data;
    private bool isAttacking;

    public void Initialize(PlayerStateMachine owner)
    {
        player = owner;

    }

    public PlayerStateMachine GetPlayer() => player;

    public void HandleAttackStart(int number)
    {
        if (isAttacking) return;
        if (player == null)
        {
            Debug.LogWarning("[WeaponCombat] Initialize(player) non chiamato prima di HandleAttackStart");
            return;
        }
        StartCoroutine(AttackRoutine(number));
    }

    public void HandleAttackEnd()
    {
        isAttacking = false;
    }

    private IEnumerator AttackRoutine(int number)
    {
        isAttacking = true;
        if (data == null)
        {
            Debug.LogWarning("[WeaponCombat] data weapon null");
            yield break;
        }

        yield return new WaitForSeconds(data.hitDelay);


        var poolManager = ServiceLocator.Get<ObjectPooler>();
        GameObject attack = poolManager.Spawn(data.attackType[number], player.transform.position, player.transform.rotation);
        var proj = attack.GetComponent<Projectile>();
        if (proj != null)
            proj.Initialize(player.p_stats, data);

        var AudioManager = ServiceLocator.Get<AudioManager>();
        if (data.swingSound != null && number < data.swingSound.Length)
            AudioManager.PlaySfx(data.swingSound[number]);

        yield return new WaitForSeconds(data.attackDuration);
        isAttacking = false;
    }
}




