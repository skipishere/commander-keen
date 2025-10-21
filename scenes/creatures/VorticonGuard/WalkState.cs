using Godot;

namespace CommanderKeen.Scenes.Creatures.Vorticon;
public partial class WalkState : VorticonBaseState
{
    public override VorticonStateMachine.VorticonStates StateType => VorticonStateMachine.VorticonStates.Walk;

    private Timer timer;

    public override void _Ready()
    {
        timer = GetNode<Timer>("WalkTimer");
    }

    private void OnTimerTimeout()
    {
        this.NextState = VorticonStateMachine.VorticonStates.Jump;
    }

    public override void StateInput(InputEvent inputEvent)
    {
    }

    public override void PhysicsProcess(double delta, float lastMovementX)
    {
        if (Character.IsOnWall())
        {
            lastMovementX = -lastMovementX;
        }
        else
        {
            lastMovementX = Character.Velocity.X > 0 ? Vector2.Right.X : Vector2.Left.X;
        }

        AnimationTree.Set("parameters/Walk/blend_position", lastMovementX);
        AnimationTree.Set("parameters/Jump/blend_position", lastMovementX);
        Character.Velocity = new Vector2(lastMovementX * Speed, Character.Velocity.Y + gravity * (float)delta);
    }

    public override void Enter()
    {
        // Walk time is around 1-4 seconds
        timer.WaitTime = random.RandfRange(1, 4);
        timer.Start();

        // Look up Keen direction
        var keen = GetTree().GetNodesInGroup("Player")[0] as Keen;
        var direction = keen.GlobalPosition.X < Character.GlobalPosition.X ? Vector2.Left.X : Vector2.Right.X;
        AnimationTree.Set("parameters/Walk/blend_position", direction);
        AnimationTree.Set("parameters/Jump/blend_position", direction);
        Character.Velocity = Character.Velocity with { X = direction };
    }
}
