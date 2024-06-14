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

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _Ready()
	{
	 	animation = GetNode<AnimationPlayer>("AnimationPlayer");
		camera = GetNode<Camera2D>("Camera2D");
		
		var collisionShape = GetNode<CollisionShape2D>("CollisionShape2D").Shape.GetRect().Size;
		width = (int)collisionShape.X/2;
		height = (int)collisionShape.Y;
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		
		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity.Y += gravity * (float)delta;
		}

		if (Input.IsActionJustPressed("move_shoot"))
		{
			var raygunInstance = raygun.Instantiate() as raygunShot;
			
			raygunInstance.GlobalPosition = this.GlobalPosition with 
			{
				X = isFacingRight ? this.GlobalPosition.X + 10 : this.GlobalPosition.X - 10,
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

		if (Input.IsActionJustPressed("move_pogo") && IsOnFloor())
		{
			//velocity.Y = JumpVelocity*1.2f;
			animation.Play("dead");
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

		// Clamp the player position to the camera limits.
		
		var xLimit = Mathf.Clamp(this.GlobalPosition.X, camera.LimitLeft+width, camera.LimitRight-width);
		var yLimit = Mathf.Clamp(this.GlobalPosition.Y, camera.LimitTop, camera.LimitBottom-height);
		if (this.GlobalPosition.Y <= camera.LimitTop && velocity.Y < 0)
		{
			Debug.Print("Hit the camera top limit");
			Velocity = Velocity with { Y = 0 };
		}
		
		this.GlobalPosition = this.GlobalPosition with { X = xLimit, Y = yLimit};
	}
}
