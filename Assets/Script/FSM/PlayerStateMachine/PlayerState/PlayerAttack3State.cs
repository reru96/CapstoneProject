using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack3State : PlayerBaseState
{
    public PlayerAttack3State(PlayerStateMachine player) : base(player) { }

    public override void Enter()
    {
        player.animator.Play("Attack3");
    }

    public override void Tick()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (input.sqrMagnitude > 0.01f)
        {
            player.rb.velocity = input.normalized * (player.agent.speed * 0.01f); 
            Quaternion targetRotation = Quaternion.LookRotation(input.normalized, Vector3.up);
            Vector3 euler = Quaternion.Slerp(player.rb.rotation, targetRotation, Time.deltaTime * player.rotationSpeed).eulerAngles;
            player.rb.MoveRotation(Quaternion.Euler(0, euler.y, 0));
        }
        else
        {
            player.rb.velocity = Vector3.zero;
        }


        AnimatorStateInfo stateInfo = player.animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.normalizedTime >= 1f)
        {
            player.SwitchState(new PlayerIdleState(player));
        }

        if (Input.GetKeyDown(InputManager.Instance.config.dodge))
        {
            player.SwitchState(new PlayerDodgeState(player));
            return;
        }
    }
}