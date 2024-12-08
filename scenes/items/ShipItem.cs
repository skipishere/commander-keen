using Godot;
using System;
using System.Diagnostics;

public partial class ShipItem : Area2D
{
    public int Points = 10000;

	[Export]
	public partType ShipPart;

	public enum partType
	{
		Battery,
		JoyStick,
		Vaccum,
		Everclear
	}

	private AudioStreamPlayer2D audioStreamPlayer;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		audioStreamPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
	}

	public void OnBodyEntered(Node2D body)
	{
		if (body is keen)
		{
			this.SetDeferred("monitoring", false);
			game_stats.Score += Points;
			Debug.Print($"Current score {game_stats.Score}");
			audioStreamPlayer.Play();
			
			var tween = GetTree().CreateTween();
			tween.Parallel().TweenProperty(this, "position", Position - new Vector2(0, 16), 1f);
			tween.Parallel().TweenProperty(this, "modulate:a", 0, 1f);
			tween.TweenCallback(Callable.From(QueueFree));
		}
	}
}
