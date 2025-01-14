using Godot;

public partial class AgroState : GargBaseState
{
    private const float Speed = 180.0f; // Same as Keen's run speed.

    private float direction = Vector2.Left.X;

    
    private const float JumpVelocity = -130f;

    private bool hasjumped = false;
    
    [Export]
    public RayCast2D wallCheckLeft;

    [Export]
    public RayCast2D wallCheckRight;

    public override GargStateMachine.GargStates StateType => GargStateMachine.GargStates.Agro;

	public override void _Process(double delta)
	{
	}

    public override void PhysicsProcess(double delta, float lastMovementX)
    {
        Character.Velocity = Character.Velocity with { X = direction * Speed };
        
        if (Character.IsOnFloor())
        {
            if ((wallCheckLeft.IsColliding() && direction == Vector2.Left.X)
                || (wallCheckRight.IsColliding() && direction == Vector2.Right.X))
            {
                NextState = GargStateMachine.GargStates.Walk;
            }

            hasjumped = false;
        }
        else
        {
            if (hasjumped)
            {
                Character.Velocity = Character.Velocity with { Y = Character.Velocity.Y + gravity * (float)delta };
            }
            else
            {
                Character.Velocity = Character.Velocity with { Y = JumpVelocity };
                hasjumped = true;
            }
        }
    }

    public override void Enter()
    {
        playback.Travel("Walk");
        hasjumped = false;

        var keen = GetTree().GetNodesInGroup("Player")[0] as Keen;
		direction = keen.GlobalPosition.X < Character.GlobalPosition.X ? Vector2.Left.X : Vector2.Right.X;
		AnimationTree.Set("parameters/Walk/blend_position", direction);
    }
}
