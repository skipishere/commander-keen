using Godot;

namespace CommanderKeen.Scenes.Creatures.TankRobot;

public partial class ShootState : TankRobotBaseState
{
    [Export]
    private PackedScene raygun;

    [Export]
    private Color raygunColor;

    public override TankRobotStateMachine.TankRobotStates StateType => TankRobotStateMachine.TankRobotStates.Shoot;
    private Timer waitTimer;

    public override void _Ready()
    {
        waitTimer = GetNode<Timer>("WaitTimer");
    }

    public override void PhysicsProcess(double delta, float lastMovementX)
    {
    }

    public void ShootFinished()
    {
        NextState = TankRobotStateMachine.TankRobotStates.Thinking;
        AnimationTree.Active = true;
    }

    public override void Enter()
    {
        waitTimer.Start();
        AnimationTree.Active = false;

        var shotPosition = Character.GlobalPosition + new Vector2(Character.Direction * 10, -2);

        var raygunInstance = raygun.Instantiate() as raygunShot;
        raygunInstance.ShotColor = raygunColor;
        raygunInstance.SetDirection(
            shotPosition,
            new Vector2(Character.Direction, 0),
            Character);
        GetTree().Root.AddChild(raygunInstance);
    }
}
