using Godot;

public partial class ShipItem : Area2D
{
    public int Points = 10000;

	[Export]
	public game_stats.ShipParts ShipPart;

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
		if (body is Keen)
		{
			this.SetDeferred("monitoring", false);
			game_stats.Score += Points;
			game_stats.SetPart(ShipPart);
			audioStreamPlayer.Play();
			signalManager.EmitSignal(nameof(SignalManager.ShipPart));
			signalManager.EmitSignal(nameof(SignalManager.ScoreChanged));
			
			var tween = GetTree().CreateTween();
			tween.Parallel().TweenProperty(this, "position", Position - new Vector2(0, 16), 1f);
			tween.Parallel().TweenProperty(this, "scale", new Vector2(4, 4), 1f);
			tween.Parallel().TweenProperty(this, "modulate:a", 0, 1f);
			tween.TweenCallback(Callable.From(QueueFree));
		}
	}
}
