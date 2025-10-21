using Godot;
using System.Diagnostics;

public partial class HazardBase : Area2D
{
    [Export]
    public bool RandomStartFrame = true;
    private static readonly RandomNumberGenerator random = new();

    [Export]
    public AnimatedSprite2D AnimatedSprite2D;

    private SignalManager signalManager;

    public override void _Ready()
    {
        signalManager = GetNode<SignalManager>("/root/SignalManager");

        if (RandomStartFrame)
        {
            var max = AnimatedSprite2D.SpriteFrames.GetFrameCount("default") - 1;
            AnimatedSprite2D.Frame = random.RandiRange(0, max);
        }
    }

    public void OnBodyEntered(Node2D body)
    {
        if (body is Keen)
        {
            Debug.WriteLine($"Keen hit a hazard - {this.Name}");
            signalManager.EmitSignal(nameof(SignalManager.KeenDead));
        }
    }
}
