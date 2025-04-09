using Godot;
using System;
using System.Diagnostics;

namespace CommanderKeen.Scenes.Creatures.Yorp;
public partial class Yorp : CharacterBody2D, ITakeDamage
{
	public const float ShovePower = 2.0f;

	private int Health = 1;

	private YorpStateMachine stateMachine = new YorpStateMachine();
	private SignalManager signalManager;

	private bool isActivated = false;
	public float lastMovementX = Vector2.Left.X;
	
	public override void _Ready()
	{
		signalManager = GetNode<SignalManager>("/root/SignalManager");
		stateMachine = GetNode<YorpStateMachine>("StateMachine");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (this.Velocity.Normalized().X != 0)
		{
			lastMovementX = this.Velocity.Normalized().X;
		}
		
		stateMachine.PhysicsProcess(delta, lastMovementX, isActivated);
		MoveAndSlide();
	}

	private void KnockedOut(Node body)
	{
		if (body is Keen)
		{
			signalManager.EmitSignal(nameof(SignalManager.KeenHitYorpEye));
			stateMachine.KnockedOut();
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

	public void ScreenExited()
	{
		isActivated = false;
		if (!Camera.CameraRect.HasPoint(this.GlobalPosition))
		{
			Debug.Print($"Yorp is out of camera bounds: {this.GlobalPosition}");
			QueueFree();
		}
	}
}
