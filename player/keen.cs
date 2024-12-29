using Godot;
using System.Diagnostics;

public partial class keen : CharacterBody2D, ITakeDamage
{
	public const float Speed = 180.0f;
		
	public AnimationTree animationTree;
	private Camera2D camera;
	private int width;
	private int height;

	private game_stats.KeyCards keyCards = 0;

	public float lastMovementX = Vector2.Right.X;

	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private StateMachine stateMachine;

	public override void _Ready()
	{
		camera = GetNode<Camera2D>("Camera2D");
		animationTree = GetNode<AnimationTree>("AnimationTree");
		stateMachine = GetNode<StateMachine>("StateMachine");

		var collisionShape = GetNode<CollisionShape2D>("CollisionShape2D").Shape.GetRect().Size;
		width = (int)collisionShape.X/2;
		height = (int)collisionShape.Y/2;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (this.Velocity.Normalized().X != 0)
		{
			lastMovementX = this.Velocity.Normalized().X;
		}
		
		stateMachine.PhysicsProcess(delta, lastMovementX);

		// if (Input.IsActionJustPressed("move_pogo") && game_stats.HasPogoStick)
		// {
		// 	this.IsPogoing = !this.IsPogoing;
		// }
		
		// if (this.IsPogoing && IsOnFloor())
		// {
		// 	//velocity.Y = JumpVelocity * (float)1.45;
		// }

		// if (Input.IsActionJustPressed("move_pogo") && IsOnFloor())
		// {
		// 	//velocity.Y = JumpVelocity*1.2f;
			
		// }

		if (MoveAndSlide())
		{
			//HandleCollision();
		}

		// Clamp the player position to the camera limits.
		var xLimit = Mathf.Clamp(this.GlobalPosition.X, camera.LimitLeft + width, camera.LimitRight - width);
		var yLimit = Mathf.Clamp(this.GlobalPosition.Y, camera.LimitTop - height, camera.LimitBottom - height);
		if (this.GlobalPosition.Y <= camera.LimitTop - height && Velocity.Y < 0)
		{
			// This is needed for maps where there is no 'roof' tiles
			Debug.Print($"Hit the camera top limit - {yLimit}");
			Velocity = Velocity with { Y = 0 };
		}
		
		this.GlobalPosition = this.GlobalPosition with { X = xLimit, Y = yLimit};
	}

	public void Shove(float direction, float delta)
	{
		// Apply a force to the player.
		var velocity = Velocity;
		if (!IsOnFloor())
		{
			velocity.Y += gravity * delta;
		}
		velocity.X = direction * Speed;
		
		Velocity = velocity;
		MoveAndSlide();
		if (IsOnWall())
		{
			Debug.WriteLine("Shove hit wall");
		}
	}

	// private void HandleCollision()
	// {
	// 	for (int i = 0; i < GetSlideCollisionCount(); i++)
	// 	{
	// 		var collision = GetSlideCollision(i);
	// 		var collider = collision.GetCollider();
	// 		//Debug.WriteLine($"Collided with {collider.GetType()}");
			
	// 		if (collider is TileMap tileMap)
	// 		{
	// 			// Handle the tilemap collision.
	// 			//Debug.WriteLine($"Collided with {tileMap.Name}");
	// 			var l2 = tileMap.ToLocal(collision.GetPosition());
	// 			tileMap.LocalToMap(l2);
	// 			var location = tileMap.LocalToMap(l2);
	// 			//Debug.WriteLine($"Collided with tilemap at {location}");
	// 			//tileMap.layer
	// 			var foo = tileMap.GetCellTileData(1, location);
	// 			if (foo != null)
	// 			{
	// 				Debug.WriteLine($"Collided with tilemap - death?");
	// 			}
	// 		}
			
			
	// 	}
	// }
	
	public void TakeDamage()
	{
		Debug.WriteLine("Keen hit!");
	}

	public void GiveKey(game_stats.KeyCards key)
	{
		keyCards |= key;
		Debug.WriteLine($"Keen has key {key}");
	}

	public bool HasKey(game_stats.KeyCards key)
	{
		return keyCards.HasFlag(key);
	}
}
