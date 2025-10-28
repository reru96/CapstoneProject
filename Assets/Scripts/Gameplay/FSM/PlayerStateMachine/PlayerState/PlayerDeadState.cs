using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;

public class PlayerDeadState : PlayerBaseState
{
    public PlayerDeadState(PlayerStateMachine player) : base(player)
    {
    }
    public override void Enter()
    {
        player.isMoving = false;
        player.agent.velocity = Vector3.zero;

        player.SetUpperBodyActive(false);

        player.animator.Play("Dead");
    }

    public override void Exit()
    {

    }

    public override void Tick()
    {
       
    }

}
