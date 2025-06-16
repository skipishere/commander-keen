using Godot;
using System;

public partial class Overworld : Node2D
{
    public override void _Ready()
    {
        if (game_stats.UsedSecretExit)
        {
            game_stats.UsedSecretExit = false;
            var signalManager = GetNode<SignalManager>("/root/SignalManager");
            var secretTeleporter = GetNode<Teleporter>("TeleporterSecret");
            signalManager.EmitSignal(nameof(SignalManager.TeleportComplete), secretTeleporter.Position, false);
            secretTeleporter.Animate();
        }
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {

    }
}
