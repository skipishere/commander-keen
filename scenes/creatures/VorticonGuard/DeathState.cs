using Godot;

namespace CommanderKeen.Scenes.Creatures.Vorticon;
public partial class DeathState : VorticonBaseState
{
	public override VorticonStateMachine.VorticonStates StateType => VorticonStateMachine.VorticonStates.Dead;

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
	}

}
