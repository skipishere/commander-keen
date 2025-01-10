using System;
using Godot;

public partial class WalkState : GargBaseState
{
	private const float Speed = 45.0f;
    public override GargStateMachine.GargStates StateType => GargStateMachine.GargStates.Walk;

	[Export]
    public RayCast2D wallCheckLeft;

	[Export]
    public RayCast2D wallCheckRight;

	private Timer timer;

	public override void _Ready()
	{
		timer = GetNode<Timer>("Timer");
	}

    private void OnTimerTimeout()
    {
        this.NextState = GargStateMachine.GargStates.Thinking;
    }

    public override void StateInput(InputEvent inputEvent)
	{
	}

    public override void PhysicsProcess(double delta, float lastMovementX)
	{
		if (player.IsOnFloor())
		{
			if (wallCheckLeft.IsColliding())
			{
				lastMovementX = Vector2.Right.X;
			}
			else if (wallCheckRight.IsColliding())
			{
				lastMovementX = Vector2.Left.X;
			}
		}

		AnimationTree.Set("parameters/Walk/blend_position", lastMovementX);
		player.Velocity = new Vector2(lastMovementX * Speed, player.Velocity.Y + gravity * (float)delta);
	}

	public override void Enter()
	{
		playback.Travel("Walk");

		// Walk time is around 1-5 seconds
		var random = new Random().Next(1, 5);
		timer.WaitTime = random;
		timer.Start();

		// Look up Keen direction
		var keen = GetTree().GetNodesInGroup("Player")[0] as keen;
		var direction = keen.GlobalPosition.X < player.GlobalPosition.X ? Vector2.Left.X : Vector2.Right.X;
		AnimationTree.Set("parameters/Walk/blend_position", direction);
		player.Velocity = player.Velocity with { X = direction };
	}

}
