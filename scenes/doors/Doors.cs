using Godot;

public partial class Doors : Node2D
{
    [Export]
    public game_stats.KeyCards Card = game_stats.KeyCards.Yellow;

    private AnimationPlayer animationPlayer;
    private SignalManager signalManager;

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        signalManager = GetNode<SignalManager>("/root/SignalManager");
    }

    public void OnArea2dBodyEntered(Node body)
    {
        if (body is Keen keen)
        {
            if (keen.HasKey(Card))
            {
                this.SetDeferred("monitoring", false);
                signalManager.EmitSignal(nameof(SignalManager.KeyCard), (int)Card, false);
                animationPlayer.Play("open");
            }
        }
    }

    public void RemoveDoor()
    {
        this.QueueFree();
    }
}
