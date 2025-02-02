using Godot;

public partial class IceCannon : Node2D
{
	[Export]
	private PackedScene iceChunk;

	[Export]
	public bool AimLeft = false;

	private int movement;

	private Vector2 startPosition;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		this.movement = AimLeft ? -1 : 1;

		startPosition = this.GlobalPosition;
		if (AimLeft)
		{
			startPosition += new Vector2(12, 0);
		}
	}

	public void FireIceChunk()
	{
		var iceChunkInstance = iceChunk.Instantiate() as IceChunk;
		iceChunkInstance.SetDirection(startPosition, new Vector2(movement, -1), new Vector2(movement * 22, -22), this);
		GetTree().Root.AddChild(iceChunkInstance);
	}
}
