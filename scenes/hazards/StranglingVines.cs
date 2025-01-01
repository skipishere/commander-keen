using Godot;
using System;
using System.Diagnostics;

public partial class StranglingVines : Area2D
{
	private SignalManager signalManager;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		signalManager = GetNode<SignalManager>("/root/SignalManager");
	}

	public void OnBodyEntered(Node2D body)
	{
		Debug.WriteLine($"Keen hit a hazard - {this.Name}");
		if (body is keen)
		{
			signalManager.EmitSignal(nameof(SignalManager.KeenDead));
		}
	}
}
