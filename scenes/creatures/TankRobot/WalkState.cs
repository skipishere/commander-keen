using Godot;

namespace CommanderKeen.Scenes.Creatures.TankRobot;

public partial class WalkState : TankRobotBaseState
{
    private const float Speed = 60f;
    public override TankRobotStateMachine.TankRobotStates StateType => TankRobotStateMachine.TankRobotStates.Move;
    private static readonly RandomNumberGenerator random = new();
    [Export]
    private PathFollow2D pathFollow2D;

    private Timer moveTimer;

    private const float ShovePower = 3.0f;

    public override void _Ready()
    {
        moveTimer = GetNode<Timer>("Timer");
    }

    private void OnTimerTimeout()
    {
        this.NextState = TankRobotStateMachine.TankRobotStates.Shoot;
    }

    public override void PhysicsProcess(double delta, float lastMovementX)
    {
        if (moveTimer.Paused)
        {
            return;
        }

        pathFollow2D.Progress += Speed * (float)delta * Character.Direction;
        
        if (pathFollow2D.ProgressRatio >= 1 || pathFollow2D.ProgressRatio <= 0)
        {
            Character.Direction *= -1;
            playback.Travel("Turn");
            moveTimer.Paused = true;
        }
        
        var velocityX = Character.Direction * Speed * (float)delta;
        
        // Test for collisions without actually moving
        var testParams = new PhysicsTestMotionParameters2D
        {
            From = Character.GlobalTransform,
            Motion = new Vector2(velocityX, 0)
        };
        
        var testResult = new PhysicsTestMotionResult2D();
        if (PhysicsServer2D.BodyTestMotion(Character.GetRid(), testParams, testResult))
        {
            var colliderId = testResult.GetColliderId();
            if (InstanceFromId(colliderId) is Keen player)
            {
                player.Shove(ShovePower * Character.Direction);
            }
        }

        AnimationTree.Set("parameters/Walk/blend_position", Character.Direction);
    }

    public override void Enter()
    {
        // Walk time is around 1-5 seconds
        moveTimer.WaitTime = random.RandfRange(1, 5);

        // Look up Keen direction
        var keen = GetTree().GetNodesInGroup("Player")[0] as Keen;
        Character.Direction = keen.GlobalPosition.X < Character.GlobalPosition.X ? Vector2.Left.X : Vector2.Right.X;

        AnimationTree.Set("parameters/Walk/blend_position", Character.Direction);
        playback.Travel("Walk");
        moveTimer.Start();
    }

    public void FinishedTurning()
    {
        moveTimer.Paused = false;
        playback.Travel("Walk");
    }
}
