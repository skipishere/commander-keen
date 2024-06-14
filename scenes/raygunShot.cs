using Godot;
using System.Diagnostics;

public partial class raygunShot : StaticBody2D
{
	const int SPEED = 300;
	private Vector2 direction;
	// Called when the node enters the scene tree for the first time.
	
	public override void _Ready()
	{
		direction = new Vector2(1, 0).Rotated(Rotation);
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var shot = MoveAndCollide(direction * SPEED * (float)delta);
		if (shot != null)
		{
			Debug.WriteLine("Shot collided");
	//		QueueFree();
		}
	}

	private void ScreenExited()
	{
		QueueFree();
	}
}
