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
}
