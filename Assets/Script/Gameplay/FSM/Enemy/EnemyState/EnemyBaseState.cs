using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EnemyBaseState : State
{
    protected EnemyStateMachine enemy;

 
    public EnemyBaseState(EnemyStateMachine enemy)
    {
        this.enemy = enemy;
    }

    public override abstract void Enter();
    public override abstract void Tick();
    public override abstract void Exit();
}
