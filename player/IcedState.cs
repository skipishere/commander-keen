using Godot;

public partial class IcedState : State
{
    public override StateMachine.KeenStates StateType => StateMachine.KeenStates.Iced;

	public override bool CanMove => false;

//	private float direction = new RandomNumberGenerator().RandfRange(-1, 1);

	public override void StateInput(InputEvent inputEvent)
	{
	}

    public override void PhysicsProcess(double delta, float lastMovementX)
	{
		Character.Velocity = Character.Velocity.MoveToward(Vector2.Zero, 1000 * (float)delta);
	}

	public override void Enter()
	{
		playback.Travel("frozen");
	}
}
