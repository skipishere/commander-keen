using Godot;
using System;

public partial class Overworld : Node2D
{
	
	private SignalManager signalManager;

	public override void _Ready()
	{
		// signalManager = GetNode<SignalManager>("/root/SignalManager");
		// if (OverworldKeen.mapPosition  != null)
		// {
		// 	GetNode<OverworldKeen>("Keen-map").Position = OverworldKeen.mapPosition.Value;
		// }
		
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
