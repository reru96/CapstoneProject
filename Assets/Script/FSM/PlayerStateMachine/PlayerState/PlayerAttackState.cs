using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
    private WeaponCombat weapon;
    private int attackNumber;
    private bool bufferNextAttack;

    private readonly string[] attackAnimations = { "Attack1", "Attack2", "Attack3" };
    private readonly float[] bufferTimes = { 0.9f, 0.7f, 1f };
    private readonly float[] endTimes = { 0.9f, 1f, 1f };

    public PlayerAttackState(PlayerStateMachine player, int attackNumber) : base(player)
    {
        this.attackNumber = attackNumber;
        weapon = player.weaponInstance;
    }

    public override void Enter()
    {
        bufferNextAttack = false;

     
        attackNumber = Mathf.Clamp(attackNumber, 0, attackAnimations.Length - 1);

     
        weapon.HandleAttackStart(attackNumber);

        player.animator.Play(attackAnimations[attackNumber]);
    }

    public override void Tick()
    {
        HandleMovement();
        HandleInput();

        AnimatorStateInfo stateInfo = player.animator.GetCurrentAnimatorStateInfo(0);

      
        if (attackNumber < 2 && stateInfo.normalizedTime >= bufferTimes[attackNumber] && bufferNextAttack)
        {
            player.SwitchState(new PlayerAttackState(player, attackNumber + 1));
            return;
        }

    
        if (stateInfo.normalizedTime >= endTimes[attackNumber] && !(attackNumber < 2 && bufferNextAttack))
        {
            weapon.HandleAttackEnd();
            player.SwitchState(new PlayerIdleState(player));
        }
    }

    private void HandleMovement()
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
    }

    private void HandleInput()
    {
        if (Input.GetKeyDown(InputManager.Instance.config.attack))
            bufferNextAttack = true;

        if (Input.GetKeyDown(InputManager.Instance.config.dodge))
        {
            weapon.HandleAttackEnd();
            player.SwitchState(new PlayerDodgeState(player));
        }
    }

    public override void Exit()
    {
        weapon.HandleAttackEnd();
    }
}
