using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{
   
    private WeaponCombat weapon;
    private int attackNumber = 1;
    private bool bufferNextAttack;
    private bool attackQueued;

    private readonly string[] attackAnimations = { "Attack1", "Attack2", "Attack3" };
    private readonly float[] bufferTimes = { 0.3f, 0.3f, 0.3f };
    private readonly float[] endTimes = { 0.9f, 1f, 1f };

    private const int UpperBodyLayerIndex = 1;

    public PlayerAttackState(PlayerStateMachine player, int attackNumber) : base(player)
    {
        this.attackNumber = Mathf.Clamp(attackNumber, 0, attackAnimations.Length - 1);
        weapon = player.weaponInstance;
    }

    public override void Enter()
    {
        bufferNextAttack = false;
        attackQueued = false;
        player.SetUpperBodyActive(true);

        if (UpperBodyLayerIndex >= 0)
        {
            player.animator.Play(attackAnimations[attackNumber], UpperBodyLayerIndex, 0f);
            player.animator.SetLayerWeight(UpperBodyLayerIndex, 1f);
        }


        weapon.HandleAttackStart(attackNumber);

        Debug.Log($"[PlayerAttackState] Enter attack {attackNumber + 1}");
    }

    public override void Tick()
    {
        HandleMovement();
        HandleInput();

        AnimatorStateInfo stateInfo = player.animator.GetCurrentAnimatorStateInfo(UpperBodyLayerIndex);

        if (attackNumber < attackAnimations.Length - 1 &&
            stateInfo.normalizedTime >= bufferTimes[attackNumber] &&
            attackQueued)
        {
            attackQueued = false;
            player.SwitchState(new PlayerAttackState(player, attackNumber + 1));
            return;
        }

        if (stateInfo.normalizedTime >= endTimes[attackNumber] &&
            !(attackNumber < attackAnimations.Length - 1 && attackQueued))
        {
            weapon.HandleAttackEnd();

            Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));
            if (input.magnitude > 0.1f)
                player.SwitchState(new PlayerMoveState(player));
            else
                player.SwitchState(new PlayerIdleState(player));
        }
    }

    private void HandleMovement()
    {
        Vector3 input = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (input.sqrMagnitude > 0.01f)
        {
            float moveSpeedMultiplier = 0.2f; 
            player.agent.velocity = input.normalized * (player.agent.speed * moveSpeedMultiplier);

            Quaternion targetRotation = Quaternion.LookRotation(input.normalized, Vector3.up);
            player.transform.rotation = Quaternion.Slerp(player.rb.rotation, targetRotation, Time.deltaTime * player.rotationSpeed);
        }
        else
        {
            player.agent.velocity = Vector3.zero;
        }
    }

    private void HandleInput()
    {
        var inputManager = ServiceLocator.Get<InputManager>();

        if (Input.GetKeyDown(inputManager.config.attack))
        {
            bufferNextAttack = true;
            attackQueued = true;
        }

        if (Input.GetKeyDown(inputManager.config.dodge))
        {
            weapon.HandleAttackEnd();
            player.SwitchState(new PlayerDodgeState(player));
        }
    }

    public override void Exit()
    {
        weapon.HandleAttackEnd();
        if (UpperBodyLayerIndex >= 0)
            player.animator.SetLayerWeight(UpperBodyLayerIndex, 0f);
        player.SetUpperBodyActive(false);

        Debug.Log($"[PlayerAttackState] Exit attack {attackNumber + 1}");
    }
}
