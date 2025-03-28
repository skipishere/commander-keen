using Godot;
using System;
using System.Diagnostics;

public partial class Yorp : CharacterBody2D, ITakeDamage
{
	[Export]
	public float Speed = 180.0f;
	
	[Export]
	public float MaxJumpVelocity = -200.0f;

	public const float ShovePower = 2.0f;

	private int Health = 1;

	public enum YorpState{
		Idle,
		WalkingLeft,
		WalkingRight,
		Thinking,
		Jumping,
		KnockedOut,
		Dying
	}

	[Export]
	private YorpState state = YorpState.Idle;
	private YorpState previousState = YorpState.Idle;

	private AnimationPlayer animationPlayer;
	private Timer knockedOutTimer;
	private VisibleOnScreenNotifier2D visibleOnScreen;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		knockedOutTimer = GetNode<Timer>("KnockedOutTimer");
		visibleOnScreen = GetNode<VisibleOnScreenNotifier2D>("VisibleOnScreen");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (!visibleOnScreen.IsOnScreen())
		{
			return;
		}

		Vector2 velocity = Velocity;
		
		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity.Y += gravity * (float)delta;
			Velocity = velocity;
		}
		else
		{
			switch (state)
			{
				case YorpState.Idle:
					//animationPlayer.Play("RESET");
					Velocity = Velocity with { X = 0 };
					break;

				case YorpState.WalkingLeft:
					//animationPlayer.Play("walk_left");
					Velocity = Velocity with { X = -1 * Speed };
					break;
					
				case YorpState.WalkingRight:
					//animationPlayer.Play("walk_right");
					Velocity = Velocity with { X = 1 * Speed };
					break;

				// case YorpState.Thinking:
				// 	animationPlayer.Play("looking");
				// 	break;

				case YorpState.Jumping:
					//Debug.Print("Yorp jumping");
					Jump();
					break;

				// case YorpState.KnockedOut:
				// 	animationPlayer.Play("dazed");
				// 	break;

				 case YorpState.Dying:
				 	animationPlayer.Play("die");

				 	break;
			}
		}

		MoveAndSlide();
	}

	private void KnockedOut(Node body)
	{
		Debug.Print("Yorp eye is sore");
		if (body is Keen keen)
		{
			Debug.Print("Yorp knocked out");
			//animationPlayer.Play("dazed");
			UpdateState(YorpState.KnockedOut);
			knockedOutTimer.Start();
		}
	}

	private void KnockedOutTimeout()
	{
		//animationPlayer.Play("RESET");
		UpdateState(YorpState.Idle);
		Debug.Print("Yorp ok");
	}

    public void TakeDamage()
    {
		Health--;
        //animationPlayer.Play("die");
		knockedOutTimer.Stop();
		UpdateState(YorpState.Dying);
    }

	private void Jump()
	{
		if (IsOnFloor())
		{
			var random = new Random();
			
			Velocity = new Vector2(Velocity.X, random.Next((int)MaxJumpVelocity, 0));
		}
	}

	public void ScreenExited()
	{
		if (!Camera.CameraRect.HasPoint(this.GlobalPosition))
		{
			Debug.Print($"Yorp is out of camera bounds: {this.GlobalPosition}");
			QueueFree();
		}
	}

	private void UpdateState(YorpState newState)
	{
		previousState = state;
		state = newState;

		if (previousState != state)
		{
			Debug.Print($"Yorp state changed from {previousState} to {state}");
			switch (state)
			{
				case YorpState.Idle:
					animationPlayer.Play("RESET");
					break;

				case YorpState.Thinking:
					animationPlayer.Play("looking");
					break;

				case YorpState.KnockedOut:
					animationPlayer.Play("dazed");
					break;

				case YorpState.Dying:
					animationPlayer.Play("die");
					break;
			}
		}
	}
}
