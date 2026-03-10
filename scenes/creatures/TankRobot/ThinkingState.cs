using Godot;

namespace CommanderKeen.Scenes.Creatures.TankRobot;

public partial class ThinkingState : TankRobotBaseState
{
    public override TankRobotStateMachine.TankRobotStates StateType => TankRobotStateMachine.TankRobotStates.Thinking;

    public override void _Ready()
    {
    }

    public override void StateInput(InputEvent inputEvent)
    {
    }

    public override void PhysicsProcess(double delta, float lastMovementX)
    {
    }

    public void ThinkingFinished()
    {
        NextState = TankRobotStateMachine.TankRobotStates.Move;
    }

    public override void Enter()
    {
        playback.Travel("Thinking");
        Character.Direction = 0;
    }
}
