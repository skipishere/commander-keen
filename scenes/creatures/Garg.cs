using Godot;

public partial class Garg : CharacterBody2D, ITakeDamage
{
	[Export]
	private int Health = 1;
	public const float Speed = 180.0f;
	public const float JumpVelocity = -315.0f;
	private bool isActivated = false;
	public AnimationTree animationTree;
	
	private SignalManager signalManager;
	private GargStateMachine stateMachine = new GargStateMachine();

	public float lastMovementX = Vector2.Left.X;

	public override void _Ready()
	{
		signalManager = GetNode<SignalManager>("/root/SignalManager");
		animationTree = GetNode<AnimationTree>("AnimationTree");
		stateMachine = GetNode<GargStateMachine>("StateMachine");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (this.Velocity.Normalized().X != 0)
		{
			lastMovementX = this.Velocity.Normalized().X;
		}
		
		stateMachine.PhysicsProcess(delta, lastMovementX, isActivated);
		
		if (MoveAndSlide())
		{
			HandleCollision();
		}
	}
	
	public override void _Process(double delta)
	{
	}

	public void BodyEntered(Node2D body)
	{
		if (body is keen)
		{
			signalManager.EmitSignal(nameof(SignalManager.KeenDead));
		}
	}

	private void HandleCollision()
	{
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var collision = GetSlideCollision(i);
			var collider = collision.GetCollider();
						
			if (collider is keen)
			{
				signalManager.EmitSignal(nameof(SignalManager.KeenDead));
			}
		}
	}

    public void TakeDamage()
    {
		stateMachine.TakeDamage();
    }

	public void OnScreenEntered()
	{
		isActivated = true;
	}

	public void OnScreenExited()
	{
		isActivated = false;

		if (!Camera.CameraRect.HasPoint(this.GlobalPosition))
		{
			QueueFree();
		}
	}
}
