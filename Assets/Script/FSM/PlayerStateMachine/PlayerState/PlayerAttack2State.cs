using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayerAttack2State : PlayerBaseState
{
    private bool bufferNextAttack = false;

    public PlayerAttack2State(PlayerStateMachine player) : base(player) { }

    public override void Enter()
    {
        player.animator.Play("Attack2");
        bufferNextAttack = false;
    }

    public override void Tick()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (input.sqrMagnitude > 0.01f)
        { 
            player.rb.velocity = input.normalized * player.agent.speed * 0.5f;
            Quaternion targetRotation = Quaternion.LookRotation(input.normalized, Vector3.up);

            Vector3 euler = Quaternion.Slerp(player.rb.rotation, targetRotation, Time.deltaTime * player.rotationSpeed).eulerAngles;
            player.rb.MoveRotation(Quaternion.Euler(0, euler.y, 0));
        }
        else
        {
            player.rb.velocity = Vector3.zero;
        }



        if (Input.GetKeyDown(InputManager.Instance.config.attack))
        {
            bufferNextAttack = true;
        }

        AnimatorStateInfo stateInfo = player.animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.normalizedTime >= 0.7f && bufferNextAttack)
        {
            player.SwitchState(new PlayerAttack3State(player));
            return;
        }

        if (stateInfo.normalizedTime >= 1f && !bufferNextAttack)
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
