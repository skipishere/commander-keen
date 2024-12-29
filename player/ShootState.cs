using Godot;

public partial class ShootState : State
{
	[Export]
	private PackedScene raygun;

    public override StateMachine.KeenStates StateType => StateMachine.KeenStates.Shoot;

	private bool hasFired = false;

	public override void StateInput(InputEvent inputEvent)
	{
	}

    public override void PhysicsProcess(double delta, float lastMovementX)
	{
		AnimationTree.Set("parameters/Shoot/blend_position", lastMovementX);
		if (game_stats.Charges > 0 && !hasFired)
			{
				game_stats.Charges--;
				var raygunInstance = raygun.Instantiate() as raygunShot;
				raygunInstance.SetDirection(player.GlobalPosition, new Vector2(lastMovementX, 0), new Vector2(lastMovementX * 16, 0), player);
				GetTree().Root.AddChild(raygunInstance);
				hasFired = true;
			}

		var stopSpeed = Speed;
		if (!player.IsOnFloor())
		{
			player.Velocity = player.Velocity with { Y = player.Velocity.Y + gravity * (float)delta };
		 	stopSpeed = Speed / 30;
		}

		player.Velocity = player.Velocity with { X = Mathf.MoveToward(player.Velocity.X, 0, stopSpeed) };
		
		if (Input.IsActionJustReleased("move_shoot"))
		{
			if (player.IsOnFloor())
			{
				NextState = StateMachine.KeenStates.Ground;
			}
			else
			{
				NextState = StateMachine.KeenStates.Air;
			}

			return;
		}
	}

	public override void Enter()
	{
		playback.Travel("Shoot");
		hasFired = false;
	}
}
