using System.Collections;
using System.Collections.Generic;
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
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
        player.rb.velocity = input.normalized * player.agent.speed;

        Quaternion targetRotation = Quaternion.LookRotation(input.normalized, Vector3.up);

       
        player.rb.MoveRotation(Quaternion.Slerp(player.rb.rotation,targetRotation,Time.deltaTime * player.rotationSpeed));

        if (input.magnitude < 0.1f)
            player.SwitchState(new PlayerIdleState(player));

        if (Input.GetKeyDown(InputManager.Instance.config.attack))
            player.SwitchState(new PlayerAttackState(player, 0));
        if (Input.GetKeyDown(InputManager.Instance.config.dodge))
            player.SwitchState(new PlayerDodgeState(player));   
    }

    public override void Exit()
    {
        player.rb.velocity = Vector3.zero;
    }
}
