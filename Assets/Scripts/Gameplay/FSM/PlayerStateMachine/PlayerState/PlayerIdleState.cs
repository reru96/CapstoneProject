using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay;
using Core;

public class PlayerIdleState : PlayerBaseState
{

    public PlayerIdleState(PlayerStateMachine player) : base(player) { }
    public override void Enter()
    {
        player.SetUpperBodyActive(false);
        player.animator.Play("Idle");
        
    }

    public override void Exit()
    {
       
    }

    public override void Tick()
    {
        var inputManager = ServiceLocator.Get<InputManager>();

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (input.magnitude > 0.1f)
            player.SwitchState(new PlayerMoveState(player));

        if (Input.GetKeyDown(inputManager.config.attack))
            player.SwitchState(new PlayerAttackState(player,0));
        if(Input.GetKeyDown(inputManager.config.dodge))    
            player.SwitchState(new PlayerDodgeState(player));
        if (Input.GetKeyDown(inputManager.config.switchWeapon))
            player.SwitchState(new PlayerSwitchWeaponState(player, 1));
    }

   
}
