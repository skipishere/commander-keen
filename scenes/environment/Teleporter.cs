using Godot;

public partial class Teleporter : Area2D
{
    [Export]
    public Teleporter Target;
    private AnimatedSprite2D animatedSprite;
    private AudioStreamPlayer2D audioStreamPlayer;
    private Timer timer;
    private bool inRange = false;
    private bool isTeleporting = false;
    private SignalManager signalManager;

    public override void _Ready()
    {
        animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        audioStreamPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
        timer = GetNode<Timer>("Timer");
        signalManager = GetNode<SignalManager>("/root/SignalManager");
    }

    public override void _Process(double delta)
    {
        if (inRange
            && Input.IsActionJustPressed("move_jump")
            && timer.IsStopped())
        {
            isTeleporting = true;
            Animate();
        }
    }

    public void Animate()
    {
        signalManager.EmitSignal(nameof(SignalManager.TeleportStart));

        audioStreamPlayer.Play();
        animatedSprite.Play("transport");
        timer.Start();
    }

    public void Idle()
    {
        animatedSprite.Play("idle");
        if (isTeleporting)
        {
            Target.Animate();
            signalManager.EmitSignal(nameof(SignalManager.TeleportComplete), Target.Position, false);
            isTeleporting = false;
        }
        else
        {
            signalManager.EmitSignal(nameof(SignalManager.TeleportComplete), this.Position, true);
        }
    }

    public void _on_body_entered(Node2D body)
    {
        if (body is OverworldKeen)
        {
            inRange = true;
        }
    }

    public void _on_body_exited(Node2D body)
    {
        if (body is OverworldKeen)
        {
            inRange = false;
        }
    }
}
