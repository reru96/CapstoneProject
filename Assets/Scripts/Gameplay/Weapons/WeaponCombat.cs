using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;

public class WeaponCombat : MonoBehaviour
{
    private PlayerStateMachine player;
    public SOWeapon data;
    private bool isAttacking;
    [SerializeField] private HitDetection hitDetection;
    [SerializeField] private Transform projectileSpawn;

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

        if (data.isRanged)
        {
            GameObject prefab = data.projectilePrefab;
            if (prefab != null)
            {
                var pooler = ServiceLocator.Get<ObjectPooler>();
                if (pooler == null)
                {
                    Debug.LogError("[WeaponCombat] ObjectPooler non trovato");
                }
                else
                {
                    Vector3 spawnPos = (projectileSpawn != null) ? projectileSpawn.position : transform.position + transform.forward * 0.5f;
                    Quaternion rot = transform.rotation;

                    GameObject projectile = pooler.Spawn(prefab, spawnPos, rot);
                    if (projectile != null)
                    {
                        var proj = projectile.GetComponent<Projectile>();
                        if (proj != null)
                            proj.Initialize(player.p_stats, data);
                    }
                }
            }
        }
        else
        {
            if (hitDetection != null)
            {
                hitDetection.Activate();
                yield return new WaitForSeconds(data.attackWindow);
                hitDetection.Deactivate();
            }
            else
            {
                Debug.LogWarning("[WeaponCombat] hitDetection non assegnato");
            }
        }

        if (data.swingSound != null)
            AudioSource.PlayClipAtPoint(data.swingSound, transform.position);

        yield return new WaitForSeconds(data.attackDuration);
        isAttacking = false;
    }
}




