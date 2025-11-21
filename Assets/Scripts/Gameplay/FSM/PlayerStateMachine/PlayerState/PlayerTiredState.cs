using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTiredState : PlayerBaseState
{
    public PlayerTiredState(PlayerStateMachine player) : base(player)
    {
    }

    public override void Enter()
    {
        player.animator.Play("Tired");
    }

    public override void Tick()
    {
        TrySwitchState(new PlayerIdleState(player));
    }
}
