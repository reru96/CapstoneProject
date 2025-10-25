using UnityEngine;

public interface IDamagable
{
    public void TakeDamage(Transform from, HitWeapon weap,float amount = 0);
}
