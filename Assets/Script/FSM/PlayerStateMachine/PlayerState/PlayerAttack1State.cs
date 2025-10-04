using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerAttack1State : PlayerBaseState
{

    public event Action OnAttackStart;
    public event Action OnAttackEnd;

    private WeaponCombat weapon;
    private bool bufferNextAttack;

    public PlayerAttack1State(PlayerStateMachine player) : base(player)
    {
        weapon = player.weaponInstance; 
    }

    public override void Enter()
    {
        player.animator.Play("Attack1");

        
        OnAttackStart += weapon.HandleAttackStart;
        OnAttackEnd += weapon.HandleAttackEnd;

        bufferNextAttack = false;

        OnAttackStart?.Invoke();
    }

    public override void Tick()
    {

        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (input.sqrMagnitude > 0.01f)
        {
            player.rb.velocity = input.normalized * (player.agent.speed * 0.01f);
            Quaternion targetRotation = Quaternion.LookRotation(input.normalized, Vector3.up);
            player.rb.MoveRotation(Quaternion.Slerp(player.rb.rotation, targetRotation, Time.deltaTime * player.rotationSpeed));
        }
        else
        {
            player.rb.velocity = Vector3.zero;
        }


        if (Input.GetKeyDown(InputManager.Instance.config.attack))
            bufferNextAttack = true;

        AnimatorStateInfo stateInfo = player.animator.GetCurrentAnimatorStateInfo(0);

       
        if (stateInfo.normalizedTime >= 0.9f)
        {
            OnAttackEnd?.Invoke(); 

            if (bufferNextAttack)
            {
                player.SwitchState(new PlayerAttack2State(player));
            }
            else
            {
                player.SwitchState(new PlayerIdleState(player));
            }
        }

        if (Input.GetKeyDown(InputManager.Instance.config.dodge))
        {
            OnAttackEnd?.Invoke();
            player.SwitchState(new PlayerDodgeState(player));
        }
    }

    public override void Exit()
    {
        OnAttackStart -= weapon.HandleAttackStart;
        OnAttackEnd -= weapon.HandleAttackEnd;
    }
}