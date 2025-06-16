using Godot;
using System;

public partial class DeathZone : Area2D
{
    private SignalManager signalManager;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        signalManager = GetNode<SignalManager>("/root/SignalManager");
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
    }

    public void OnDeathZoneBodyEntered(Node2D body)
    {
        if (body is Keen)
        {
            signalManager.EmitSignal(nameof(SignalManager.KeenDead));
        }
    }
}
