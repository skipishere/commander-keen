using System.Diagnostics;
using Godot;

public partial class OverworldKeen : CharacterBody2D
{
	public const float Speed = 100.0f;
	
	public static Vector2? mapPosition;

	private Camera2D camera;
	private int width;
	private int height;
	private bool isTeleporting = false;

	private SignalManager signalManager;

	private AnimatedSprite2D animatedSprite;

	public override void _Ready()
	{
	 	animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		camera = GetNode<Camera2D>("Camera2D");
		
		var collisionShape = GetNode<CollisionShape2D>("CollisionShape2D").Shape.GetRect().Size;
		width = (int)collisionShape.X/2;
		height = (int)collisionShape.Y;
		
		signalManager = GetNode<SignalManager>("/root/SignalManager");
		signalManager.TeleportStart += OnTeleportStart;
		signalManager.TeleportComplete += OnTeleportComplete;
		signalManager.EnteringLevel += OnEnteringLevel;

		Debug.Print($"OverworldKeen has {mapPosition}.");
		if (mapPosition.HasValue)
		{
			Debug.Print($"Saved Coords x {mapPosition.Value.X} y {mapPosition.Value.Y}");
			this.Position = mapPosition.Value;
		}
	}

	public void _on_animated_sprite_2d_tree_exited()
	{
		signalManager.TeleportStart -= OnTeleportStart;
		signalManager.TeleportComplete -= OnTeleportComplete;
		signalManager.EnteringLevel -= OnEnteringLevel;
	}

	private void OnTeleportStart()
	{
		// TODO State machine
		this.Visible = false;
		this.isTeleporting = true;
	}

	private void OnTeleportComplete(Vector2 position, bool finished)
	{
		// TODO State machine
		animatedSprite.Play("down");
		animatedSprite.Frame = 0;
		this.Position = position;
		
		this.Visible = finished;
		this.isTeleporting = !finished;
	}

	private void OnEnteringLevel(string levelResource)
	{
		Debug.Print($"Coords x {Position.X} y {Position.Y}");
		mapPosition = Position;
	}

	public override void _PhysicsProcess(double delta)
	{
		if (isTeleporting)
		{
			return;
		}

		Vector2 velocity = Velocity;

		Vector2 direction = Input.GetVector("move_left", "move_right", "map_up", "map_down");
		if (direction != Vector2.Zero)
		{
			velocity.X = direction.X * Speed;
			velocity.Y = direction.Y * Speed;

			// TODO fix animation when direction is changed while moving
			// Add state machine.
			string animationToPlay = string.Empty;
			
			if (direction.Y > 0)
			{
				animationToPlay = "down";
			}
			else if (direction.Y < 0)
			{
				animationToPlay = "up";
			}
			else if (direction.X > 0)
			{
				animationToPlay = "right";
			}
			else if (direction.X < 0)
			{
				animationToPlay = "left";
			}

			if (!animatedSprite.IsPlaying())
			{
				animatedSprite.Frame = 1;
			}

			if ((!string.IsNullOrEmpty(animationToPlay) 
			&& animatedSprite.Animation.ToString() != animationToPlay)
			|| !animatedSprite.IsPlaying())
			{
				animatedSprite.Play(animationToPlay);
			}
		}
		else
		{
			velocity = velocity with 
			{
				 X = Mathf.MoveToward(Velocity.X, 0, Speed),
				 Y = Mathf.MoveToward(Velocity.Y, 0, Speed)
			};
		}
		
		if (direction == Vector2.Zero)
		{
			animatedSprite.Stop();
		}
		
		Velocity = velocity;
		MoveAndSlide();

		// Clamp the player position to the camera limits.
		var xLimit = Mathf.Clamp(this.GlobalPosition.X, camera.LimitLeft+width, camera.LimitRight-width);
		var yLimit = Mathf.Clamp(this.GlobalPosition.Y, camera.LimitTop, camera.LimitBottom-height);
		
		this.GlobalPosition = this.GlobalPosition with { X = xLimit, Y = yLimit};
	}
}
