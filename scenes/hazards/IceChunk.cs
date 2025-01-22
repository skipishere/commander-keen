using Godot;
using System;

public partial class IceChunk : StaticBody2D
{
	private const int Speed = 50;
	private Vector2 direction;
	private GodotObject origin;

	private SignalManager signalManager;

	public void SetDirection(Vector2 globalPosition, Vector2 direction, Vector2 offset, GodotObject origin)
	{
		this.GlobalPosition = globalPosition + direction + offset;
		this.direction = direction;
		this.Transform = this.Transform with { X = this.Transform.X * (direction.X > 0 ? 1 : -1) };
		this.origin = origin;
	}
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		signalManager = GetNode<SignalManager>("/root/SignalManager");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		this.GlobalPosition += this.direction * Speed * (float)delta;

		var collision = MoveAndCollide(direction * Speed * (float)delta);
		if (collision != null)
		{
			if (collision.GetCollider() is Keen keen)
			{
				signalManager.EmitSignal(nameof(SignalManager.KeenFrozen));
				QueueFree();
			}
			else
			{
				// Animate fracture then QueueFree()
				// For now, just QueueFree()
				QueueFree();
			}
		}
	}
}
