using Godot;
using System;
using System.Diagnostics;

public partial class vorticon_guard : CharacterBody2D, ITakeDamage
{
	[Export]
	private int Health = 3;

	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
	public const float Speed = 180.0f;
	public const float JumpVelocity = -315.0f;
	private bool isActivated = false;


	private AnimatedSprite2D animation;
	private CollisionShape2D collisionShape;

	private Timer hitTimer;
	private SignalManager signalManager;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animation = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
		hitTimer = GetNode<Timer>("HitTimer");
		signalManager = GetNode<SignalManager>("/root/SignalManager");
	}

    private void HitTimeout()
    {
        (animation.Material as ShaderMaterial).SetShaderParameter("line_thickness", 0);
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
		if (!isActivated)
		{
			return;
		}

		var velocity = Velocity;

		if (Health <= 0)
		{
			velocity.Y += gravity * (float)delta;
			Velocity = velocity;
			MoveAndSlide();
			return;
		}


		if (!IsOnFloor())
		{
			velocity.Y += gravity * (float)delta;
		}
		else
		{
			velocity.Y = JumpVelocity;
		}

		Velocity = velocity;
		if (MoveAndSlide())
		{
			HandleCollision();
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
		(animation.Material as ShaderMaterial).SetShaderParameter("line_thickness", 1);
		hitTimer.Start();

		Health--;
		Debug.WriteLine("Vorticon Guard took damage! Health: " + Health);
		if (Health <= 0)
		{
			Debug.WriteLine("Vorticon Guard defeated!");
			animation.Play("die");
			this.SetCollisionLayerValue(4, false);
			this.SetCollisionMaskValue(2, false);
		}
	}

	public void _on_screen_entered()
	{
		Debug.WriteLine("Vorticon Guard on screen");
		isActivated = true;
	}

	public void OnScreenExited()
	{
		Debug.WriteLine("Vorticon Guard off screen");
		isActivated = false;

		if (!Camera.CameraRect.HasPoint(this.GlobalPosition))
		{
			Debug.Print($"Vorticon Guard is out of camera bounds: {this.GlobalPosition}");
			QueueFree();
		}
	}

	// private void _on_Area2D_body_entered(object body)
	// {
	// 	if (body is keen)
	// 	{
	// 		signalManager.EmitSignal(nameof(SignalManager.KeenDead));
	// 	}
	// }
}
