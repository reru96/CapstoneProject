using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackTrigger : MonoBehaviour
{
    private ZombieAnimation zombieParent;
    [SerializeField] private GameObject hitbox;

    private void Awake()
    {
        zombieParent = GetComponentInParent<ZombieAnimation>();
        hitbox.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            zombieParent.StartAttack(other);
            hitbox.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            zombieParent.StopAttack(other);
            hitbox.SetActive(false);
        }
    }
}
