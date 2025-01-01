using Godot;

public partial class AirState : State
{
	private const float JumpVelocity = -315.0f;

    public override StateMachine.KeenStates StateType => StateMachine.KeenStates.Air;

    public override void _Ready()
	{
	}

	public override void PhysicsProcess(double delta, float lastMovementX)
	{
		if (Input.IsActionJustReleased("move_jump") && player.Velocity.Y < 0)
		{
			player.Velocity = player.Velocity with { Y = 0 };
		}

		if (player.IsOnFloor())
		{
			NextState = StateMachine.KeenStates.Ground;
			return;
		}

		AnimationTree.Set("parameters/Fall/blend_position", lastMovementX);
		AnimationTree.Set("parameters/Jump/blend_position", lastMovementX);
		
		var movement = Input.GetAxis("move_left", "move_right");
		player.Velocity = new Vector2(
			movement * Speed,
			player.Velocity.Y + gravity * (float)delta);
	}

	public override void Enter()
	{
		if (Input.IsActionPressed("move_jump") && player.IsOnFloor())
		{
			playback.Travel("Jump");
			player.Velocity = player.Velocity with { Y = JumpVelocity };
		}
		else
		{
			playback.Travel("Fall");
			player.Velocity = player.Velocity with { Y = 40 };
		}
	}
}
