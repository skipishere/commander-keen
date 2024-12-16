using Godot;
using System;
using System.Diagnostics;

public partial class KeyCard : Area2D
{
	[Export]
    public game_stats.KeyCards Card = game_stats.KeyCards.Yellow;

	private AudioStreamPlayer2D audioStreamPlayer;
	private SignalManager signalManager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		audioStreamPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		signalManager = GetNode<SignalManager>("/root/SignalManager");
	}

	public void OnBodyEntered(Node2D body)
	{
		if (body is keen keen)
		{
			this.SetDeferred("monitoring", false);
			keen.GiveKey(Card);
			audioStreamPlayer.Play();
			signalManager.EmitSignal(nameof(SignalManager.KeyCard), (int)Card, true);
			
			var tween = GetTree().CreateTween();
			tween.Parallel().TweenProperty(this, "position", Position - new Vector2(0, 16), 1f);
			tween.Parallel().TweenProperty(this, "modulate:a", 0, 1f);
			tween.TweenCallback(Callable.From(QueueFree));
		}
	}
}
