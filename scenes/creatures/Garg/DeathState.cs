using Godot;

public partial class DeathState : GargBaseState
{
	public override GargStateMachine.GargStates StateType => GargStateMachine.GargStates.Dead;

	public override bool CanMove => false;

	public override void StateInput(InputEvent inputEvent)
	{
	}

    public override void PhysicsProcess(double delta, float lastMovementX)
	{
		Character.Velocity = new Vector2 (0, Character.Velocity.Y + gravity * (float)delta);
	}

	public override void Enter()
	{
		playback.Travel("Die");
		// Garg has a slight jump when killed.
		Character.Velocity = new Vector2 (0, -75);
	}

}
