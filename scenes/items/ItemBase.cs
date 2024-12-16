using Godot;
using System;
using System.Diagnostics;

public partial class ItemBase : Area2D
{
	[Export]
    public int Points = 0;

	private AudioStreamPlayer2D audioStreamPlayer;
	private Label label;

	private SignalManager signalManager;

	public override void _Ready()
	{
		audioStreamPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		label = GetNode<Label>("Label");
		label.Visible = false;
		label.Text = Points.ToString();
		signalManager = GetNode<SignalManager>("/root/SignalManager");
	}

	public void OnBodyEntered(Node2D body)
	{
		if (body is keen)
		{
			label.Visible = true;
			this.SetDeferred("monitoring", false);
			game_stats.Score += Points;
			signalManager.EmitSignal(nameof(SignalManager.ScoreChanged));
			audioStreamPlayer.Play();
			
			var tween = GetTree().CreateTween();
			tween.Parallel().TweenProperty(this, "position", Position - new Vector2(0, 16), 1f);
			tween.Parallel().TweenProperty(this, "modulate:a", 0, 1f);
			tween.TweenCallback(Callable.From(QueueFree));
		}
	}
}
