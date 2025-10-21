using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slash : BaseAttack
{
    public Slash(AttackType attackType) : base(attackType)
    {
        this.AttackType = AttackType.Slashing;
    }
}
