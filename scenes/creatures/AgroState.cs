using Godot;

public partial class AgroState : GargBaseState
{
    private const float Speed = 180.0f; // Same as Keen's run speed.
    private float direction = Vector2.Left.X;
    
    [Export]
    public RayCast2D wallCheck;

    public override GargStateMachine.GargStates StateType => GargStateMachine.GargStates.Agro;

	public override void _Process(double delta)
	{
	}

    public override void PhysicsProcess(double delta, float lastMovementX)
    {
        if (wallCheck.IsColliding())
        {
            NextState = GargStateMachine.GargStates.Walk;
        }

        player.Velocity = player.Velocity with { X = direction * Speed };
    }

    public override void Enter()
    {
        playback.Travel("Walk");

        var keen = GetTree().GetNodesInGroup("Player")[0] as keen;
		direction = keen.GlobalPosition.X < player.GlobalPosition.X ? Vector2.Left.X : Vector2.Right.X;
		AnimationTree.Set("parameters/Walk/blend_position", direction);
    }
}
