using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    public PlayerMoveState(PlayerStateMachine player) : base(player) { }

    public override void Enter()
    {
        player.animator.Play("Run");
    }

    public override void Tick()
    {
        var inputManager = ServiceLocator.Get<InputManager>();
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        player.agent.velocity = input * player.agent.speed;

        Quaternion targetRotation = Quaternion.LookRotation(input.normalized, Vector3.up);
        player.transform.rotation = Quaternion.Slerp(player.rb.rotation,targetRotation,Time.deltaTime * player.rotationSpeed);

        if (input.magnitude < 0.1f)
            player.SwitchState(new PlayerIdleState(player));

        if (Input.GetKeyDown(inputManager.config.attack))
            player.SwitchState(new PlayerAttackState(player, 0));
        if (Input.GetKeyDown(inputManager.config.dodge))
            player.SwitchState(new PlayerDodgeState(player));
        if (Input.GetKeyDown(inputManager.config.switchWeapon))
            player.SwitchState(new PlayerSwitchWeaponState(player, 1));
    }

    public override void Exit()
    {
        player.agent.velocity = Vector3.zero;
    }
}
