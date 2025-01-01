using Godot;

public partial class DeadKeen : CharacterBody2D
{
	private SignalManager signalManager;

	// Death animation speed (different from player speed)
	public const float Speed = 90000.0f;
		
	private Vector2 endPosition;
	
	public override void _Ready()
	{
		signalManager = GetNode<SignalManager>("/root/SignalManager");
	}

	private void OnWaitTimeout()
	{
		endPosition = new Vector2(
			100 * new RandomNumberGenerator().RandfRange(-1, 1),
			-300
		);
	}

    public override void _PhysicsProcess(double delta)
    {
		this.Velocity = this.Velocity.MoveToward(endPosition, (float)delta * Speed);
		MoveAndSlide();
    }

	public void OffScreen()
	{
		signalManager.EmitSignal(nameof(SignalManager.ExitingLevel));
		QueueFree();
	}
}
