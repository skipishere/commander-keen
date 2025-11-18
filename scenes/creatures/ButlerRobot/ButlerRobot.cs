using Godot;

public partial class ButlerRobot : StaticBody2D, ITakeDamage
{
    public const float Speed = 0.842f;

    public const float ShovePower = 2.0f;

    private int direction = 1;
    private AnimatedSprite2D animatedSprite2D;
    private RayCast2D rayCastLeft;
    private RayCast2D rayCastRight;

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
            animatedSprite2D.Play(direction > 0 ? "right" : "left");
        }

        // Store Y position before any movement to prevent vertical displacement
        var lockedY = this.GlobalPosition.Y;
        
        // ButlerBot moves horizontally only, no gravity
        var velocity = new Vector2(direction * Speed, 0);

        var result = MoveAndCollide(velocity);
        if (result?.GetCollider() is Keen player)
        {
            // Determine push direction based on relative positions
            float pushDirection = Mathf.Sign(player.GlobalPosition.X - this.GlobalPosition.X);
            if (pushDirection == 0) pushDirection = direction;
            
            // Test if Keen can actually move in the push direction
            var testParams = new PhysicsTestMotionParameters2D
            {
                From = player.GlobalTransform,
                Motion = new Vector2(pushDirection * 2.0f, 0)
            };
            var testResult = new PhysicsTestMotionResult2D();
            
            // If the push direction is blocked, reverse it
            if (PhysicsServer2D.BodyTestMotion(player.GetRid(), testParams, testResult))
            {
                // Check if there's ANY space in the natural direction
                float travelDistance = testResult.GetTravel().Length();
                
                // Only reverse if there's very little space (less than 1 pixel)
                if (travelDistance < 1.0f)
                {
                    pushDirection = -pushDirection;
                }
            }
            
            player.Shove(ShovePower * pushDirection, (float)delta);
        }
        else if (result?.GetCollider() is TileMapLayer wall)
        {
            direction *= -1;
            animatedSprite2D.Play("turn");
        }
        
        // Force Y position to remain constant - cannot be pushed vertically
        this.GlobalPosition = new Vector2(this.GlobalPosition.X, lockedY);
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
