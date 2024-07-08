using Godot;
using System;
using System.Diagnostics;

public partial class ButlerRobot : CharacterBody2D
{
	public const float Speed = 0.842f;

	private int direction = 1;
	private AnimatedSprite2D animatedSprite2D;
	private RayCast2D rayCastLeft;
	private RayCast2D rayCastRight;

	public override void _Ready()
	{
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		rayCastLeft = GetNode<RayCast2D>("RayCastLeft");
		rayCastRight = GetNode<RayCast2D>("RayCastRight");
	}

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		if (animatedSprite2D.IsPlaying() && animatedSprite2D.Animation == "turn")
		{
			return;
		}

		if (!rayCastRight.IsColliding() && !rayCastLeft.IsColliding())
		{
			direction *= -1;
			animatedSprite2D.Play("turn");
		}
		else
		{
			if (!animatedSprite2D.IsPlaying())
			{
				animatedSprite2D.Play(direction > 0 ? "right": "left");
			}
		}

		velocity.X = direction * Speed;
		
		Velocity = velocity;
		
		var result = MoveAndCollide(velocity);
		if (result?.GetCollider() is keen player)
		{
			// Todo push keen
			//Debug.Print("Player hit");
		}
		else if (result?.GetCollider() is TileMap wall)
		{
			direction *= -1;
			animatedSprite2D.Play("turn");
		}
	}
}
