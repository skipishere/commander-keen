using Godot;
using static game_stats;

public partial class Overworld : Node2D
{
    public override void _Ready()
    {
        if (UsedSecretExit)
        {
            UsedSecretExit = false;
            var signalManager = GetNode<SignalManager>("/root/SignalManager");
            var secretTeleporter = GetNode<Teleporter>("TeleporterSecret");
            signalManager.EmitSignal(nameof(SignalManager.TeleportComplete), secretTeleporter.Position, false);
            secretTeleporter.Animate();
        }

        if (CollectedParts == (ShipParts)15)
        {
            var endgame = GetNode<TileMapLayer>("TileMapLayers/Endgame");
            endgame.Visible = true;

            var keen = GetNode<OverworldKeen>("Keen-map");
            keen.Visible = false;
            keen.ProcessMode = ProcessModeEnum.Disabled;
        }
    }
}
