using System.Diagnostics;
using Godot;

public partial class DeadState : State
{
	[Export]
	private PackedScene deadKeen;

    public override StateMachine.KeenStates StateType => StateMachine.KeenStates.Dead;

	public override bool CanMove => false;

	private float direction = new RandomNumberGenerator().RandfRange(-1, 1);

	public override void StateInput(InputEvent inputEvent)
	{
	}

    public override void PhysicsProcess(double delta, float lastMovementX)
	{
		player.Velocity = Vector2.Zero;
	}

	public override void Enter()
	{
		var deadInstance = deadKeen.Instantiate() as DeadKeen;
		deadInstance.Position = new Vector2(player.Position.X, player.Position.Y + 4);
		playback.Travel("Hidden");
		
		GetTree().Root.AddChild(deadInstance);
	}
}
