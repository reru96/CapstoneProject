using Core;
using Gameplay;
using UnityEngine;

public class PlayerMoveState : PlayerBaseState
{
    public PlayerMoveState(PlayerStateMachine player) : base(player) { }

    public override void Enter()
    {
        player.isMoving = true;
        player.animator.SetBool("isMoving", player.isMoving);
        player.animator.SetFloat("Speed", player.agent.velocity.magnitude);
    }

    public override void Tick()
    {
        var inputManager = ServiceLocator.Get<InputManager>();
        Vector3 input = GetMovementInput();

        Vector3 targetVelocity = input.normalized * player.p_data.moveSpeed;

        player.agent.velocity = Vector3.SmoothDamp(
            player.agent.velocity,
            targetVelocity,
            ref player.currentVelocity,
            player.accelerationTime
        );

        if (input.sqrMagnitude > 0.01f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(input.normalized, Vector3.up);
            player.transform.rotation = Quaternion.Slerp(player.transform.rotation, targetRotation, Time.deltaTime * player.p_data.rotSpeed);
        }

        player.animator.SetFloat("Speed", player.agent.velocity.magnitude);

        if (input.magnitude < 0.1f)
            TrySwitchState(new PlayerIdleState(player));

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

    public override void Exit()
    {
        player.isMoving = false;
        player.agent.velocity = Vector3.zero;
        player.currentVelocity = Vector3.zero;
        player.animator.SetBool("isMoving", false);
        player.animator.SetFloat("Speed", 0f);
    }

}
