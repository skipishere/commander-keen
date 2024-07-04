using Godot;
using System;

public partial class OverworldKeen : CharacterBody2D
{
	public const float Speed = 100.0f;
	
	private Camera2D camera;
	private int width;
	private int height;

	private int animationLoopOut;
	private bool isTeleporting = false;
	private readonly int animationLoopOutSet = 5;

	private AnimatedSprite2D animatedSprite;

	public override void _Ready()
	{
	 	animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		camera = GetNode<Camera2D>("Camera2D");
		
		var collisionShape = GetNode<CollisionShape2D>("CollisionShape2D").Shape.GetRect().Size;
		width = (int)collisionShape.X/2;
		height = (int)collisionShape.Y;
	}

	private void OnTeleportStart()
	{
		// TODO State machine
		this.Visible = false;
		this.isTeleporting = true;
	}

	private void OnTeleportComplete()
	{
		// TODO State machine
		this.Visible = true;
		this.isTeleporting = false;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (isTeleporting)
		{
			return;
		}

		Vector2 velocity = Velocity;

		Vector2 direction = Input.GetVector("move_left", "move_right", "map_up", "map_down");
		if (direction != Vector2.Zero)
		{
			animationLoopOut = animationLoopOutSet;
			velocity.X = direction.X * Speed;
			velocity.Y = direction.Y * Speed;
		}
		else
		{
			velocity.X = Mathf.MoveToward(Velocity.X, 0, Speed);
			velocity.Y = Mathf.MoveToward(Velocity.Y, 0, Speed);
		}

		if (velocity.Y > 0)
		{
			animatedSprite.Play("down");
		}
		else if (velocity.Y < 0)
		{
			animatedSprite.Play("up");
		}
		else if (velocity.X > 0)
		{
			animatedSprite.Play("right");
		}
		else if (velocity.X < 0)
		{
			animatedSprite.Play("left");
		}
		else if (animationLoopOut > 0)
		{
			// Bit of a hack to keep the animation looping for a few frames.
			animationLoopOut--;
		}
		else
		{
			animatedSprite.Stop();
		}

		Velocity = velocity;
		MoveAndSlide();

		// Clamp the player position to the camera limits.		
		var xLimit = Mathf.Clamp(this.GlobalPosition.X, camera.LimitLeft+width, camera.LimitRight-width);
		var yLimit = Mathf.Clamp(this.GlobalPosition.Y, camera.LimitTop, camera.LimitBottom-height);
		
		this.GlobalPosition = this.GlobalPosition with { X = xLimit, Y = yLimit};
	}
}
