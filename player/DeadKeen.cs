using System;
using System.Diagnostics;
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
		Debug.WriteLine($"Position: {Position}");
		
		//Debug.WriteLine($"End Position: {endPosition}");
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
        //base._PhysicsProcess(delta);
		Debug.WriteLine($"End Position: {endPosition}");
		this.Velocity = this.Velocity.MoveToward(endPosition, (float)delta * Speed);
		MoveAndSlide();
		//Debug.WriteLine($"Position: {Position}");
    }
    public override void _Process(double delta)
	{
	}

	public void OffScreen()
	{
		signalManager.EmitSignal(nameof(SignalManager.ExitingLevel));
		QueueFree();
	}
}
