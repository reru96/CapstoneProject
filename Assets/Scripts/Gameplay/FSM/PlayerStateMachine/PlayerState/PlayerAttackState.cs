using System.Collections;
using System.Collections.Generic;
using Core;
using Gameplay;
using UnityEngine;

public class PlayerAttackState : PlayerBaseState
{

    private WeaponCombat weapon;
    private int attackNumber = 1;
    private bool attackQueued;
    private bool canQueue;

    private readonly string[] attackAnimations = { "Attack1", "Attack2", "Attack3" };
    private readonly float[] bufferTimes = { 0.3f, 0.3f, 0.3f };
    private readonly float[] endTimes = { 1f, 1f, 1f }; 

    private const int UpperBodyLayerIndex = 1;
    private const float rotationSpeedDuringAttack = 8f;
    private const float attackMoveMultiplier = 0.25f; 
    private const float queueAcceptWindow = 0.35f; 

    private float queueWindowStartTime = 0f;

    public PlayerAttackState(PlayerStateMachine player, int attackNumber) : base(player)
    {
        this.attackNumber = Mathf.Clamp(attackNumber, 0, attackAnimations.Length - 1);
        weapon = player.weaponInstance;
    }

    public override void Enter()
    {
        attackQueued = false;
        canQueue = false;
        queueWindowStartTime = 0f;

        player.SetUpperBodyActive(true);

        if (UpperBodyLayerIndex >= 0)
        {
            player.animator.Play(attackAnimations[attackNumber], UpperBodyLayerIndex, 0f);
            player.animator.SetLayerWeight(UpperBodyLayerIndex, 1f);
        }

        if (player.agent != null)
            player.agent.updatePosition = true;

        weapon.HandleAttackStart(attackNumber);

        Debug.Log($"[PlayerAttackState] Enter attack {attackNumber + 1}");
    }

    public override void Tick()
    {
        HandleMovementDuringAttack();
        HandleInput();

        AnimatorStateInfo stateInfo = player.animator.GetCurrentAnimatorStateInfo(UpperBodyLayerIndex);

        if (attackNumber < attackAnimations.Length - 1 &&
            stateInfo.normalizedTime >= bufferTimes[attackNumber] &&
            !canQueue)
        {
            canQueue = true;
            queueWindowStartTime = Time.time;
        }

        if (canQueue && Time.time - queueWindowStartTime > queueAcceptWindow)
            canQueue = false;

        if (attackNumber < attackAnimations.Length - 1 &&
            attackQueued &&
            stateInfo.normalizedTime >= 1f)
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

    private void HandleMovementDuringAttack()
    {
      
        Vector3 rawInput = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        if (rawInput.sqrMagnitude > 0.01f && player.agent != null)
        {
            Vector3 worldDir = player.transform.TransformDirection(rawInput.normalized);
            player.agent.velocity = worldDir * (player.agent.speed * attackMoveMultiplier);

           
            Quaternion targetRotation = Quaternion.LookRotation(worldDir, Vector3.up);
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, Time.deltaTime * rotationSpeedDuringAttack);
        }
        else if (player.agent != null)
        {
           
            player.agent.velocity = Vector3.zero;
        }
    }

    private void HandleInput()
    {
        var inputManager = ServiceLocator.Get<InputManager>();

        if (Input.GetKeyDown(inputManager.config.attack))
        {
            if (attackNumber < attackAnimations.Length - 1)
            {
               
                attackQueued = true;
                Debug.Log("[PlayerAttackState] Attack input buffered/queued");
            }
        }

        if (Input.GetKeyDown(inputManager.config.dodge))
        {
            weapon.HandleAttackEnd();
            player.SwitchState(new PlayerDodgeState(player));
        }

        if (Input.GetKeyDown(inputManager.config.switchWeapon))
        {
            weapon.HandleAttackEnd();
            player.SwitchState(new PlayerSwitchWeaponState(player, -1));
        }
    }

    public override void Exit()
    {
        weapon.HandleAttackEnd();

        if (UpperBodyLayerIndex >= 0)
            player.animator.SetLayerWeight(UpperBodyLayerIndex, 0f);

        player.SetUpperBodyActive(false);

        if (player.agent != null)
            player.agent.updatePosition = true;

        Debug.Log($"[PlayerAttackState] Exit attack {attackNumber + 1}");
    }
}
