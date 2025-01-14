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
				raygunInstance.SetDirection(Character.GlobalPosition, new Vector2(lastMovementX, 0), new Vector2(lastMovementX * 16, 0), Character);
				GetTree().Root.AddChild(raygunInstance);
				hasFired = true;
			}

		var stopSpeed = Speed;
		if (!Character.IsOnFloor())
		{
			Character.Velocity = Character.Velocity with { Y = Character.Velocity.Y + gravity * (float)delta };
		 	stopSpeed = Speed / 30;
		}

		Character.Velocity = Character.Velocity with { X = Mathf.MoveToward(Character.Velocity.X, 0, stopSpeed) };
		
		if (Input.IsActionJustReleased("move_shoot"))
		{
			if (Character.IsOnFloor())
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
