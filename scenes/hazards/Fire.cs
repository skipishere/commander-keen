using Godot;

public partial class Fire : Node2D
{
    private SignalManager signalManager;

    public override void _Ready()
    {
        signalManager = GetNode<SignalManager>("/root/SignalManager");
    }

    public void OnBodyEntered(Node2D body)
    {
        if (body is Keen)
        {
            signalManager.EmitSignal(nameof(SignalManager.KeenDead));
        }
    }
}
