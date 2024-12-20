using Godot;
using System;
using System.Diagnostics;

public partial class keen : CharacterBody2D, ITakeDamage
{
	[Export]
	private PackedScene raygun;

	public const float Speed = 180.0f;
	public const float JumpVelocity = -315.0f;

	public bool IsJumping { get; set; }
	public bool IsShooting { get; set; }
	public bool IsPogoing { get; set; }
	public bool IsOnGround => IsOnFloor();
	public bool IsOnIce => groundType == GroundType.Ice;

	//private AnimationPlayer animation;
	private AnimationTree animationTree;
	private Camera2D camera;
	private int width;
	private int height;

	private game_stats.KeyCards keyCards = 0;

	private SignalManager signalManager;
	private RayCast2D rayCast;

	private float lastMovementX = Vector2.Right.X;

	private enum GroundType
	{
		Normal,
		
		// Can change direction but there is a delay before stopping.
		Slippery,
		
		// Can't change direction, keep on moving in given direction unless you jump and change direction.
		Ice,
	}

	private GroundType groundType = GroundType.Normal;

	// Get the gravity from the project settings to be synced with RigidBody nodes.
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	public override void _Ready()
	{
	 	//animation = GetNode<AnimationPlayer>("AnimationPlayer");
		camera = GetNode<Camera2D>("Camera2D");
		animationTree = GetNode<AnimationTree>("AnimationTree");

		var collisionShape = GetNode<CollisionShape2D>("CollisionShape2D").Shape.GetRect().Size;
		width = (int)collisionShape.X/2;
		height = (int)collisionShape.Y/2;

		signalManager = GetNode<SignalManager>("/root/SignalManager");
		signalManager.KeenDead += TakeDamage;
		rayCast = GetNode<RayCast2D>("RayCast2D");
	}

	public void OnTreeExited()
	{
		signalManager.KeenDead -= TakeDamage;
	}

	public override void _PhysicsProcess(double delta)
	{
		var idle = this.Velocity == Vector2.Zero;
		if (this.Velocity.Normalized().X != 0)
		{
			lastMovementX = this.Velocity.Normalized().X;
		}

		animationTree.Set("parameters/Idle/blend_position", lastMovementX);
		animationTree.Set("parameters/Fall/blend_position", lastMovementX);
		animationTree.Set("parameters/Walk/blend_position", lastMovementX);
		animationTree.Set("parameters/Jump/blend_position", lastMovementX);
		animationTree.Set("parameters/Shoot/blend_position", lastMovementX);

		if (this.Visible == false)
		{
			// Exiting the level either by door or teleporter.
			return;
		}

		Vector2 velocity = Velocity;
		
		// Add the gravity.
		if (!IsOnFloor())
		{
			velocity.Y += gravity * (float)delta;
			//groundType = GroundType.Normal;
		}
		else if (rayCast.IsColliding())
		{
			var collider = rayCast.GetCollider();
			if (collider is TileMapLayer tileMapLayer)
			{
				 var local_col_point = tileMapLayer.ToLocal(rayCast.GetCollisionPoint());
				 var cell_coords = tileMapLayer.LocalToMap(local_col_point);
				
				 var cellData = tileMapLayer.GetCellTileData(cell_coords);
				 try
				 {
				 	// We sometimes get null on the line below, possibly due to tilemaplayer issues the camera has had to deal with?
				 	var slide = cellData.GetCustomData("Slide");
					groundType = slide.ToString() switch
					{
						"1" => GroundType.Slippery,
						"2" => GroundType.Ice,
						_ => GroundType.Normal
					};
				 }
				 catch (Exception e)
				 {
					 Debug.WriteLine($"Error: {e.Message}");
					 groundType = GroundType.Normal;
				 }
			}
		}

		if (Input.IsActionJustPressed("move_shoot") && game_stats.Charges > 0)
		{
			this.IsShooting = true;
			this.IsJumping = false;
			this.IsPogoing = false;

			game_stats.Charges--;
			var raygunInstance = raygun.Instantiate() as raygunShot;
			raygunInstance.SetDirection(this.GlobalPosition, new Vector2(lastMovementX, 0), new Vector2(lastMovementX > 0 ? 16 : -16, 0), this);
			GetTree().Root.AddChild(raygunInstance);
		}
		else if (Input.IsActionJustReleased("move_shoot"))
		{
			this.IsShooting = false;
		}

		if (Input.IsActionJustPressed("move_pogo") && game_stats.HasPogoStick)
		{
			this.IsPogoing = !this.IsPogoing;
		}
		
		if (this.IsPogoing && IsOnFloor())
		{
			velocity.Y = JumpVelocity * (float)1.45;
		}

		// Handle Jump.
		if (Input.IsActionJustPressed("move_jump") && IsOnFloor())
		{
			this.IsJumping = true;
			velocity.Y = JumpVelocity;
		}

		// Handle Jump height, work in progress.
		if (Input.IsActionJustReleased("move_jump") && velocity.Y < 0)
		{
			velocity.Y = 0;
		}

		if (Input.IsActionJustPressed("move_pogo") && IsOnFloor())
		{
			//velocity.Y = JumpVelocity*1.2f;
			
		}

		if (Input.IsMouseButtonPressed(MouseButton.Right))
		{
			GetTree().Paused = !GetTree().Paused;
			game_stats.Charges+=10;
		}

		// TODO add state machine to handle the player states.
		// eg animation.Play("dead");

		// Get the input direction and handle the movement/deceleration.
		Vector2 direction = Input.GetVector("move_left", "move_right", "ui_up", "ui_down");
		if (direction != Vector2.Zero && (groundType != GroundType.Slippery || !IsOnFloor()))
		{
			if (groundType != GroundType.Ice || !IsOnFloor())
			{
				velocity.X = direction.X * Speed;
			}
		}
		else
		{
			var toSpeed = 0f;
			if (groundType == GroundType.Ice && IsOnFloor())
			{
				toSpeed = lastMovementX * Speed;
			}
			else if (groundType == GroundType.Slippery && IsOnFloor())
			{
				// TODO deal with falling from an ice ledge onto ice, as the velocity should not just hit 0.
				if ( direction.X == 0)
				{
					// With no player input, the player will slow down to 1/2 max speed or slower.
					toSpeed = Mathf.Clamp(Velocity.X, -Speed / 2, Speed / 2);
				}
				else
				{
					// this is a bit more interesting, changing direction can additively slow the player down to 0.
					toSpeed = Mathf.Clamp(Velocity.X + (direction.X * Speed * 0.05f), -Speed, Speed);
				}
			}
			
			velocity.X = Mathf.MoveToward(Velocity.X, toSpeed, Speed);
		}

		Velocity = velocity;
		if (MoveAndSlide())
		{
			//HandleCollision();
		}

		// Clamp the player position to the camera limits.
		var xLimit = Mathf.Clamp(this.GlobalPosition.X, camera.LimitLeft + width, camera.LimitRight - width);
		var yLimit = Mathf.Clamp(this.GlobalPosition.Y, camera.LimitTop - height, camera.LimitBottom - height);
		if (this.GlobalPosition.Y <= camera.LimitTop - height && velocity.Y < 0)
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
		Debug.WriteLine("Keen defeated!");
		//animation.Play("dead");
		//this.SetCollisionMaskValue(1, false);
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

	public void StoppedJumping()
	{
		this.IsJumping = false;
	}
}
