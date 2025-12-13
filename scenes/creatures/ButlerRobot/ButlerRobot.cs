using Godot;

public partial class ButlerRobot : StaticBody2D, ITakeDamage
{
    private const float Speed = 0.842f;
    private const float ShovePower = 2.0f;
    private const int KeenCollisionLayer = 2;
    private const float PassThroughDistance = 24f; // Distance to move past Keen

    private int direction = 1;
    private AnimatedSprite2D animatedSprite2D;
    private RayCast2D rayCastLeft;
    private RayCast2D rayCastRight;
    private Timer hitTimer;
    private bool isPassingThrough;
    private float passThroughStartX;
    private ulong lastTurnFrame;

    public override void _Ready()
    {
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        rayCastLeft = GetNode<RayCast2D>("RayCastLeft");
        rayCastRight = GetNode<RayCast2D>("RayCastRight");
        hitTimer = GetNode<Timer>("HitTimer");
    }

    public override void _PhysicsProcess(double delta)
    {
        // Update animation based on ledge detection
        if (!rayCastRight.IsColliding() && !rayCastLeft.IsColliding())
        {
            direction *= -1;
            animatedSprite2D.Play("turn");
            lastTurnFrame = Engine.GetPhysicsFrames();
            
            // End pass-through when turning at ledge
            if (isPassingThrough)
            {
                EndPassThrough();
            }
        }
        else if (!animatedSprite2D.IsPlaying() || animatedSprite2D.Animation == "turn")
        {
            animatedSprite2D.Play(direction > 0 ? "right" : "left");
        }

        // Check if pass-through has expired (moved past Keen)
        if (isPassingThrough)
        {
            float distanceMoved = Mathf.Abs(GlobalPosition.X - passThroughStartX);
            if (distanceMoved >= PassThroughDistance)
            {
                EndPassThrough();
            }
        }
        
        // Store Y position to prevent vertical displacement
        var lockedY = this.GlobalPosition.Y;
        var currentX = this.GlobalPosition.X;
        
        // ButlerBot moves horizontally only, no gravity
        var velocity = new Vector2(direction * Speed, 0);

        // Test for collisions without actually moving
        var testParams = new PhysicsTestMotionParameters2D
        {
            From = this.GlobalTransform,
            Motion = velocity
        };
        var testResult = new PhysicsTestMotionResult2D();
        
        if (PhysicsServer2D.BodyTestMotion(this.GetRid(), testParams, testResult))
        {
            var collider = testResult.GetCollider();
            if (collider != null)
            {
                var colliderId = testResult.GetColliderId();
                var colliderObject = GodotObject.InstanceFromId(colliderId);
                
                if (colliderObject is Keen player)
                {
                    HandleKeenCollision(player, delta);
                }
                else if (colliderObject is TileMapLayer)
                {
                    // Only process wall collision if not recently turned (prevent rapid turn loop)
                    ulong currentFrame = Engine.GetPhysicsFrames();
                    if (currentFrame - lastTurnFrame >= 5)
                    {
                        HandleWallCollision();
                    }
                }
            }
        }
        
        // Move by setting position directly - StaticBody2D cannot be pushed
        this.GlobalPosition = new Vector2(currentX + velocity.X, lockedY);
    }

    private void HandleKeenCollision(Keen player, double delta)
    {
        if (isPassingThrough)
        {
            return;
        }

        // Test if Keen can be pushed forward
        if (CanPushKeen(player, direction))
        {
            player.Shove(ShovePower * direction, (float)delta);
        }
        else
        {
            // Keen is blocked by a wall - pass through
            StartPassThrough();
        }
    }

    private void HandleWallCollision()
    {
        direction *= -1;
        animatedSprite2D.Play("turn");
        lastTurnFrame = Engine.GetPhysicsFrames();
        
        // End pass-through immediately when hitting wall and turning around
        if (isPassingThrough)
        {
            EndPassThrough();
        }
    }

    private bool CanPushKeen(Keen keen, float pushDirection)
    {
        var testParams = new PhysicsTestMotionParameters2D
        {
            From = keen.GlobalTransform,
            Motion = new Vector2(pushDirection * ShovePower, 0)
        };
        var testResult = new PhysicsTestMotionResult2D();
        return !PhysicsServer2D.BodyTestMotion(keen.GetRid(), testParams, testResult);
    }

    private void StartPassThrough()
    {
        SetCollisionMaskValue(KeenCollisionLayer, false);
        isPassingThrough = true;
        passThroughStartX = GlobalPosition.X;
    }

    private void EndPassThrough()
    {
        SetCollisionMaskValue(KeenCollisionLayer, true);
        isPassingThrough = false;
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
