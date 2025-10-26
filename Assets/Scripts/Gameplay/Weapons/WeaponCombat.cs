using System;
using System.Collections;
using System.Collections.Generic;
using Core;
using UnityEngine;
using UnityEngine.XR;
using static Codice.Client.Common.Connection.AskCredentialsToUser;

public class WeaponCombat : MonoBehaviour
{
    private PlayerStateMachine player;
    public SOWeapon data;
    private bool isAttacking;
    [SerializeField] private HitDetection hitDetection;
    [SerializeField] private Transform projectileSpawn;

    private void Awake()
    {
        if (hitDetection == null && data != null && data.hitDetectionPrefab != null)
        {
            GameObject go = Instantiate(data.hitDetectionPrefab, transform);
            hitDetection = go.GetComponent<HitDetection>();
        }

        if (projectileSpawn == null)
        {
            Transform t = transform.Find("ProjectileSpawn") ?? transform.Find("Muzzle");
            if (t != null) projectileSpawn = t;
        }
    }

    public void Initialize(PlayerStateMachine owner)
    {
        player = owner;

        if (hitDetection != null)
        {
            hitDetection.SetBaseValue(data.baseDamage);
            hitDetection.SetLayerMask(data.hitLayerMask);
            hitDetection.SetOwner(this, player);
        }
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

        if (data.isRanged)
        {
            GameObject prefab = data.projectilePrefab;
            if (prefab != null)
            {
                var pooler = ServiceLocator.Get<ObjectPooler>();
                if (pooler != null)
                {
                    Vector3 spawnPos = (projectileSpawn != null) ? projectileSpawn.position : transform.position + transform.forward * 0.5f;
                    Quaternion rot = transform.rotation;

                    GameObject projectile = pooler.Spawn(prefab, spawnPos, rot);
                    if (projectile != null)
                    {
                        var proj = projectile.GetComponent<Projectile>();
                        if (proj != null && player != null)
                            proj.Initialize(player.p_stats, data);
                    }
                }
                else
                {
                    Debug.LogError("[WeaponCombat] ObjectPooler non trovato");
                }
            }
        }
        else
        {
            if (hitDetection != null && data.attackType != null && number < data.attackType.Length)
            {
                var reference = data.attackType[number];
                if (reference != null)
                {
                    var refCol = reference.GetComponent<Collider>();
                    var targetCol = hitDetection.GetTriggerCollider();

                    if (refCol != null && targetCol != null && refCol.GetType() == targetCol.GetType())
                    {
                        if (refCol is SphereCollider refSphere && targetCol is SphereCollider targetSphere)
                        {
                            targetSphere.radius = refSphere.radius;
                            targetSphere.center = refSphere.center;
                        }
                        else if (refCol is BoxCollider refBox && targetCol is BoxCollider targetBox)
                        {
                            targetBox.size = refBox.size;
                            targetBox.center = refBox.center;
                        }
                        else if (refCol is CapsuleCollider refCap && targetCol is CapsuleCollider targetCap)
                        {
                            targetCap.radius = refCap.radius;
                            targetCap.height = refCap.height;
                            targetCap.center = refCap.center;
                            targetCap.direction = refCap.direction;
                        }
                    }
                }

                hitDetection.enabled = true;
                yield return new WaitForSeconds(data.attackWindow);
                hitDetection.enabled = false;
            }
        }

        var AudioManager = ServiceLocator.Get<AudioManager>();
        if (data.swingSound != null && number < data.swingSound.Length)
            AudioManager.PlaySfx(data.swingSound[number]);

        if (data.particleSystem != null && number < data.particleSystem.Length)
        {
            Instantiate(data.particleSystem[number],
                        transform.position + transform.forward * 0.5f,
                        Quaternion.identity);
        }

        yield return new WaitForSeconds(data.attackDuration);
        isAttacking = false;
    }
}




