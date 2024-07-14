using Godot;
using System;
using System.Diagnostics;

public partial class RaygunItem : Area2D
{
	private AudioStreamPlayer2D audioStreamPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		audioStreamPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnBodyEntered(Node2D body)
	{
		Debug.Print($"Collided with {body.GetType()}");
		if (body is keen)
		{
			this.SetDeferred("monitoring", false);
			game_stats.Charges+=5;
			audioStreamPlayer.Play();
			
			var tween = GetTree().CreateTween();
			tween.Parallel().TweenProperty(this, "position", Position - new Vector2(0, 16), 1f);
			tween.Parallel().TweenProperty(this, "modulate:a", 0, 1f);
			tween.TweenCallback(Callable.From(QueueFree));
		}
	}
}
