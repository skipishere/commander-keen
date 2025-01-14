using Godot;

public partial class ButlerRobot : CharacterBody2D, ITakeDamage
{
	public const float Speed = 0.842f;

	public const float ShovePower = 2.0f;

	private int direction = 1;
	private AnimatedSprite2D animatedSprite2D;
	private RayCast2D rayCastLeft;
	private RayCast2D rayCastRight;
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private Timer hitTimer;

	public override void _Ready()
	{
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		rayCastLeft = GetNode<RayCast2D>("RayCastLeft");
		rayCastRight = GetNode<RayCast2D>("RayCastRight");
		hitTimer = GetNode<Timer>("HitTimer");
	}

	public override void _PhysicsProcess(double delta)
	{
		if (animatedSprite2D.IsPlaying() && animatedSprite2D.Animation == "turn")
		{
			return;
		}

		if (!rayCastRight.IsColliding() && !rayCastLeft.IsColliding())
		{
			direction *= -1;
			animatedSprite2D.Play("turn");
		}
		else if (!animatedSprite2D.IsPlaying())
		{
			animatedSprite2D.Play(direction > 0 ? "right": "left");
		}

		var velocity = new Vector2 (direction * Speed, 0);
		
		var result = MoveAndCollide(velocity);
		if (result?.GetCollider() is Keen player)
		{
			player.Shove(ShovePower * direction, (float)delta);
		}
		else if (result?.GetCollider() is TileMapLayer wall)
		{
			direction *= -1;
			animatedSprite2D.Play("turn");
		}
	}

	public void TakeDamage()
	{
		(animatedSprite2D.Material as ShaderMaterial).SetShaderParameter("line_thickness", 0.6);
		hitTimer.Start();
	}

	private void HitTimeout()
    {
        (animatedSprite2D.Material as ShaderMaterial).SetShaderParameter("line_thickness", 0);
    }
}
