using Godot;
using System;
using System.Diagnostics;

public partial class keen : CharacterBody2D
{
	[Export]
	private PackedScene raygun;

	public const float Speed = 180.0f;
	public const float JumpVelocity = -315.0f;
	private AnimationPlayer animation;
	private Camera2D camera;
	private int width;
	private int height;
	private bool isFacingRight = true;

	private SignalManager signalManager;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _Ready()
	{
	 	animation = GetNode<AnimationPlayer>("AnimationPlayer");
		camera = GetNode<Camera2D>("Camera2D");
		
		var collisionShape = GetNode<CollisionShape2D>("CollisionShape2D").Shape.GetRect().Size;
		width = (int)collisionShape.X/2;
		height = (int)collisionShape.Y;

		signalManager = GetNode<SignalManager>("/root/SignalManager");
		signalManager.KeenDead += TakeDamage;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		
		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity.Y += gravity * (float)delta;
		}

		if (Input.IsActionJustPressed("move_shoot") && game_stats.Charges > 0)
		{
			game_stats.Charges--;
			var raygunInstance = raygun.Instantiate() as raygunShot;
			
			var shotOffset = 6 * (isFacingRight ? 1 : -1);
			raygunInstance.GlobalPosition = this.GlobalPosition with 
			{
				X = this.GlobalPosition.X + shotOffset,
				Y = this.GlobalPosition.Y 
			};
			raygunInstance.RotationDegrees = isFacingRight ? 0 : 180;
			GetTree().Root.AddChild(raygunInstance);
		}
		
		// Handle Jump.
		if (Input.IsActionJustPressed("move_jump") && IsOnFloor())
		{
			velocity.Y = JumpVelocity;
			animation.Stop();
		}

		// Handle Jump height, work in progress.
		if (Input.IsActionJustReleased("move_jump") && velocity.Y < 0)
		{
			velocity.Y = 0;
		}

		if (Input.IsActionJustPressed("move_pogo") && IsOnFloor())
		{
			//velocity.Y = JumpVelocity*1.2f;
			animation.Play("dead");
			game_stats.Charges+=10;
		}

		if (Input.IsMouseButtonPressed(MouseButton.Right))
		{
			GetTree().Paused = !GetTree().Paused;
		}

		// TODO add state machine to handle the player states.
		// eg animation.Play("dead");

		// Get the input direction and handle the movement/deceleration.
		// As good practice, you should replace UI actions with custom gameplay actions.
		Vector2 direction = Input.GetVector("move_left", "move_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
		}

		if (velocity.X > 0)
		{
			animation.Play("walk_right");
			isFacingRight = true;
		}
		else if (velocity.X < 0)
		{
			animation.Play("walk_left");
			isFacingRight = false;
		}
		else
		{
			animation.Stop();
		}
		
		Velocity = velocity;
		MoveAndSlide();
		HandleColliosion();

		// Clamp the player position to the camera limits.		
		var xLimit = Mathf.Clamp(this.GlobalPosition.X, camera.LimitLeft+width, camera.LimitRight-width);
		var yLimit = Mathf.Clamp(this.GlobalPosition.Y, camera.LimitTop, camera.LimitBottom-height);
		if (this.GlobalPosition.Y <= camera.LimitTop && velocity.Y < 0)
		{
			// This is needed for maps where there is no 'roof' tiles
			Debug.Print("Hit the camera top limit");
			Velocity = Velocity with { Y = 0 };
		}
		
		this.GlobalPosition = this.GlobalPosition with { X = xLimit, Y = yLimit};
	}

	private void HandleColliosion()
	{
		for (int i = 0; i < GetSlideCollisionCount(); i++)
		{
			var collision = GetSlideCollision(i);
			var collider = collision.GetCollider();
			if (collider is vorticon_guard)
			{
				Debug.WriteLine("Vorticon collision detected!");
				TakeDamage();
			}
		}
	}

	public void TakeDamage()
	{
		Debug.WriteLine("Keen defeated!");
		animation.Play("dead");
		this.SetCollisionLayerValue(1, false);
	}
}
