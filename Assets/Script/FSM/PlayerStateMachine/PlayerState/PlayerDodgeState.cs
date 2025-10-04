using System.Collections;
using System.Collections.Generic;
using JetBrains.Annotations;
using UnityEngine;

public class PlayerDodgeState : PlayerBaseState
{
    public PlayerDodgeState(PlayerStateMachine player) : base(player)
    {
    }
    public override void Enter()
    {
        player.animator.Play("Dodge");
        player.isInvincible = true;
    }

    public override void Tick()
    {

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (input.sqrMagnitude > 0.01f)
        {
  
            player.rb.velocity = input.normalized * player.agent.speed;

            Quaternion targetRotation = Quaternion.LookRotation(input.normalized, Vector3.up);
            player.rb.MoveRotation(Quaternion.Slerp(player.rb.rotation, targetRotation, Time.deltaTime * player.rotationSpeed));
        }
        else
        {
            player.rb.velocity = Vector3.zero;
        }

        AnimatorStateInfo stateInfo = player.animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.normalizedTime >= 1f) 
        {
            player.isInvincible = false;

            if (input.sqrMagnitude > 0.01f)
            {
                
                player.SwitchState(new PlayerMoveState(player));
            }
            else
            {
               
                player.SwitchState(new PlayerIdleState(player));
            }
        }

     
    }

    public override void Exit()
    {
        base.Exit();
        player.isInvincible = false;
    }
}


