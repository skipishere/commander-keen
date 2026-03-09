using System;
using Godot;

namespace CommanderKeen.Scenes.Creatures.TankRobot;
public partial class ShootState : TankRobotBaseState
{
    public override TankRobotStateMachine.TankRobotStates StateType => TankRobotStateMachine.TankRobotStates.Shoot;

    public override void _Ready()
    {
    }

    public override void StateInput(InputEvent inputEvent)
    {
    }

    public override void PhysicsProcess(double delta, float lastMovementX)
    {
    }

    public void ShootFinished()
    {
        NextState = TankRobotStateMachine.TankRobotStates.Thinking;
    }

    public override void Enter()
    {
        playback.Travel("Shoot");
        AnimationTree.Set("parameters/Shoot/blend_position", Character.Direction);
    }
}
