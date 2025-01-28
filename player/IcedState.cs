using Godot;

public partial class IcedState : State
{
	[Export]
	public Timer IcedLength { get; set; }

    public override StateMachine.KeenStates StateType => StateMachine.KeenStates.Iced;

	public override bool CanMove => false;

	public override void StateInput(InputEvent inputEvent)
	{
	}

    public override void PhysicsProcess(double delta, float lastMovementX)
	{
		//Character.Velocity = Character.Velocity.MoveToward(Vector2.Zero, 1000 * (float)delta);

		var movement = 0f;
		if (Character.IsOnFloor())
		{
			// Slide, need to check speed
			movement = Mathf.Clamp(Character.Velocity.X, -Speed / 2, Speed / 2);
		}
		else
		{
			Character.Velocity = new Vector2(
				Character.Velocity.X,
				Character.Velocity.Y + gravity * (float)delta);
		}

	}

	public void Thaw()
	{
		playback.Travel("thaw");
	}

	public void Melted()
	{
		NextState = StateMachine.KeenStates.Ground;
	}

	public override void Enter()
	{
		//Todo determine if Keen was hit by a left or right moving ice chunk
		playback.Travel("frozen");
		Character.Velocity = new Vector2(200, -400);
		IcedLength.Start();
	}
}
