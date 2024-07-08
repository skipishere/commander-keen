using System.Diagnostics;
using Godot;

public partial class LevelPortal : Area2D
{

	[Export]
	public PackedScene Target;

	private bool inRange = false;
	private SignalManager signalManager;

	public override void _Ready()
	{
		signalManager = GetNode<SignalManager>("/root/SignalManager");
	}

	public override void _Process(double delta)
	{
		if (inRange && Input.IsActionJustPressed("move_jump"))
		{
			Debug.Print($"Level Portal activated - {Target.ResourcePath}");
			signalManager.EmitSignal(nameof(SignalManager.EnteringLevel));
			GetTree().ChangeSceneToPacked(Target);
		}
	}

	public void OnBodyEntered(Node2D body)
	{
		Debug.Print("Level Portal reached");
		if (body is OverworldKeen)
		{
			inRange = true;
		}
	}

	public void OnBodyExited(Node2D body)
	{
		Debug.Print("Level Portal exited");
		if (body is OverworldKeen)
		{
			inRange = false;
		}
	}
}
