using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using PlasticGui;
using UnityEngine;

public class PlayerDodgeState : PlayerBaseState
{
   
    private static readonly float animationEndThreshold = 0.95f; 
    private readonly int dodgeHash = Animator.StringToHash("Dodge");

    public PlayerDodgeState(PlayerStateMachine player) : base(player) { }

    public override void Enter()
    {
        player.isDodging = true;
        player.isInvincible = true;

        player.animator.SetBool("isDodging", true);
        player.agent.velocity = Vector3.zero;
    }

    public override void Tick()
    {
        HandleMovement();

        AnimatorStateInfo stateInfo = player.animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.shortNameHash == dodgeHash && stateInfo.normalizedTime >= animationEndThreshold)
        {
            EndDodge();
        }
    }

    private void HandleMovement()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (input.sqrMagnitude > 0.01f)
        {
            Vector3 direction = input.normalized;
            player.agent.velocity = direction * player.agent.speed;

            Quaternion targetRotation = Quaternion.LookRotation(direction, Vector3.up);
            player.transform.rotation = Quaternion.Slerp(player.rb.rotation, targetRotation, Time.deltaTime * player.rotationSpeed);
        }
        else
        {
            player.agent.velocity = Vector3.zero;
        }
    }

    private void EndDodge()
    {
        player.isDodging = false;
        player.isInvincible = false;
        player.animator.SetBool("isDodging", false);

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (input.sqrMagnitude > 0.01f)
        {
            player.isMoving = true;
            player.SwitchState(new PlayerMoveState(player));
        }
        else
        {
            player.SwitchState(new PlayerIdleState(player));
        }
    }

    public override void Exit()
    {
        player.isDodging = false;
        player.isInvincible = false;
        player.animator.SetBool("isDodging", false);

    }
}

