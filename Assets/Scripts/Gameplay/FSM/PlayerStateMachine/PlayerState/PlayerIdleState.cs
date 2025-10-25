using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;
using static Codice.Client.Commands.WkTree.WorkspaceTreeNode;
using static Codice.Client.Common.WebApi.WebApiEndpoints;

public class PlayerIdleState : PlayerBaseState
{

    public PlayerIdleState(PlayerStateMachine player) : base(player) { }

    public override void Enter()
    {
        player.isMoving = false;
        player.agent.velocity = Vector3.zero;

        player.SetUpperBodyActive(false);

        player.animator.SetBool("isMoving", false);
        player.animator.SetFloat("Speed", 0f);
    }

    public override void Exit()
    { 

    }

    public override void Tick()
    {
        var inputManager = ServiceLocator.Get<InputManager>();
        Vector3 input = GetMovementInput();

        if (input.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(input.normalized, Vector3.up);
            player.transform.rotation = Quaternion.Slerp(player.rb.rotation, targetRotation, Time.deltaTime * player.rotationSpeed);
        }

        if (input.magnitude > 0.1f)
            TrySwitchState(new PlayerMoveState(player));

        if (Input.GetKeyDown(inputManager.config.attack))
        {
            player.isAttacking = true;
            TrySwitchState(new PlayerAttackState(player, 0));
        }
           

        if (Input.GetKeyDown(inputManager.config.dodge))
        {
            player.isDodging = true;
            TrySwitchState(new PlayerDodgeState(player));
        }
            

        if (Input.GetKeyDown(inputManager.config.switchWeapon))
            TrySwitchState(new PlayerSwitchWeaponState(player, 1));
    }
}

