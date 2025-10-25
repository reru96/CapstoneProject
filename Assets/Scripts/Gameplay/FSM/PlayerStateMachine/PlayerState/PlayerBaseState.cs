using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBaseState : State
{
    protected PlayerStateMachine player;

    public PlayerBaseState(PlayerStateMachine player)
    {
        this.player = player;
    }

    public override void Enter(){}
    
    public override void Tick(){}
    
    public override void Exit(){}

    protected Vector3 GetMovementInput()
    {
        return new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
    }

    protected void TrySwitchState(PlayerBaseState newState)
    {
        if (player.CurrentState.GetType() != newState.GetType())
            player.SwitchState(newState);
    }
}
