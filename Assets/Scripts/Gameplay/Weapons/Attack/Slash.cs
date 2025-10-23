using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : BaseAttack
{
    public Slash(DamageType attackType) : base(attackType)
    {
        this.AttackType = DamageType.Slashing;
    }
}
