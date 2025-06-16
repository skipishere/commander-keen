using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Godot;

public partial class StartLevelState : State
{
    public override bool CanMove => false;
    public override StateMachine.KeenStates StateType => StateMachine.KeenStates.StartLevel;

    public override void PhysicsProcess(double delta, float lastMovementX)
    {
    }

    public void DelayInput()
    {
        NextState = StateMachine.KeenStates.Ground;
    }

    public override void Enter()
    {
        Character.Velocity = new Vector2(0, gravity);
    }
}
