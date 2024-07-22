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
			var secretTeleporter = GetNode<teleporter>("TeleporterSecret");
			signalManager.EmitSignal(nameof(SignalManager.TeleportComplete), secretTeleporter.Position, false);
			secretTeleporter.Animate();
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{

	}

	// public void OnArea2dBodyEntered(Node2D body)
	// {
	// 	if (body is OverworldKeen)
	// 	{
	// 		keenPosition =  body.Position;
	// 		GetTree().ChangeSceneToFile("res://scenes/ck1lv01.tscn");
	// 	}
	// }

}
