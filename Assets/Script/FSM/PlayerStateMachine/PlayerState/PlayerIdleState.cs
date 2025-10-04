using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerIdleState : PlayerBaseState
{

    public PlayerIdleState(PlayerStateMachine player) : base(player) { }
    public override void Enter()
    {
        player.animator.Play("Idle");
        
    }

    public override void Exit()
    {
       
    }

    public override void Tick()
    {

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (input.magnitude > 0.1f)
            player.SwitchState(new PlayerMoveState(player));

        if (Input.GetKeyDown(InputManager.Instance.config.attack))
            player.SwitchState(new PlayerAttack1State(player));
        if(Input.GetKeyDown(InputManager.Instance.config.dodge))    
            player.SwitchState(new PlayerDodgeState(player));
    }

   
}
