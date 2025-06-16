using Godot;

public partial class PogoStick : Area2D
{
    private AudioStreamPlayer2D audioStreamPlayer;

    private SignalManager signalManager;

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
            audioStreamPlayer.Play();
            if (game_stats.HasPogoStick == game_stats.PogoStickState.No)
            {
                game_stats.HasPogoStick = game_stats.PogoStickState.Gained;
                signalManager.EmitSignal(nameof(SignalManager.PogoStick));
            }

            var tween = GetTree().CreateTween();
            tween.Parallel().TweenProperty(this, "scale", new Vector2(4, 4), 1f);
            tween.Parallel().TweenProperty(this, "modulate:a", 0, 1f);
            tween.TweenCallback(Callable.From(QueueFree));
        }
    }
}
