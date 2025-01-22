using Godot;
using System;
using System.Diagnostics;

public partial class IceCannon : Node2D
{
	[Export]
	private PackedScene iceChunk;

	[Export]
	public bool AimLeft = false;

	private int movement;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.movement = AimLeft ? -1 : 1;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void FireIceChunk()
	{
		var iceChunkInstance = iceChunk.Instantiate() as IceChunk;
		iceChunkInstance.SetDirection(this.GlobalPosition, new Vector2(movement, -1), new Vector2(movement * 22, -22), this);
		GetTree().Root.AddChild(iceChunkInstance);

		Debug.WriteLine($"Firing ice chunk to the {(AimLeft ? "left" : "right")}");
	}
}
