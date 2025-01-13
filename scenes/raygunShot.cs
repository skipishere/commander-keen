using Godot;
using System.Diagnostics;

public partial class raygunShot : StaticBody2D
{
	const int SPEED = 300;
	private Vector2 direction;
	
	private Sprite2D zap;
	private GpuParticles2D fireParticles;

	private CollisionShape2D collisionShape;
	private GodotObject origin;
	
	public override void _Ready()
	{
		//direction = new Vector2(1, 0);//.Rotated(Rotation);
		fireParticles = GetNode<GpuParticles2D>("Fire");
		zap = GetNode<Sprite2D>("Zap");
		collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
	}

	public void SetDirection(Vector2 globalPosition, Vector2 direction, Vector2 offset, GodotObject origin)
	{
		this.GlobalPosition = globalPosition + direction + offset;
		// {
		// 	X = globalPosition.X + (direction.X);// * shotOffset),
		// };

		this.direction = direction;
		this.Transform  = this.Transform with { X = this.Transform.X * (direction.X > 0 ? 1 : -1) };
		this.origin = origin;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		var modifier = this.zap.Visible ? 0.1f : 1.0f;
		var shot = MoveAndCollide(direction * SPEED * modifier * (float)delta);

		if (shot != null && shot.GetCollider() != origin)
		{	
			zap.FlipH = direction.X < 0;
			collisionShape.Disabled = true;
			//direction = shot.GetTravel() with { X = (float)(shot.GetTravel().X * 0.25)};
			
			if (shot.GetCollider() is ITakeDamage creature)
			{
				Debug.Print($"Collided with {creature.GetType()}");
				creature.TakeDamage();
				QueueFree();
			}
			else
			{
				Debug.Print("Collided with something that doesn't take damage");
				fireParticles.Emitting = false;
				
				zap.Visible = true;
				
				var tween = GetTree().CreateTween();
				tween.Parallel().TweenProperty(this, "scale", new Vector2(0, 0), 0.5f);
				tween.Parallel().TweenProperty(this, "modulate:a", 0, 0.5f);
				tween.TweenCallback(Callable.From(QueueFree));
			}
		}
	}

	public void ShotHit()
	{
		QueueFree();
	}

	private void ScreenExited()
	{
		QueueFree();
	}
}
