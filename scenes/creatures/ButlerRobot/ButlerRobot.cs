using Godot;

public partial class ButlerRobot : StaticBody2D, ITakeDamage
{
    private const float Speed = 0.842f;
    private const float ShovePower = 2.0f;
    private const ulong PassThroughFrames = 30; // 0.5 seconds at 60 FPS
    private const int KeenCollisionLayer = 2;

    private int direction = 1;
    private AnimatedSprite2D animatedSprite2D;
    private RayCast2D rayCastLeft;
    private RayCast2D rayCastRight;
    private Timer hitTimer;
    private bool isPassingThrough;
    private ulong passThroughEndFrame;

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

        // Check if pass-through has expired
        if (isPassingThrough && Engine.GetPhysicsFrames() >= passThroughEndFrame)
        {
            EndPassThrough();
        }
        
        // Store Y position before any movement to prevent vertical displacement
        var lockedY = this.GlobalPosition.Y;
        
        // ButlerBot moves horizontally only, no gravity
        var velocity = new Vector2(direction * Speed, 0);

        var result = MoveAndCollide(velocity);
        
        if (result?.GetCollider() is Keen player)
        {
            HandleKeenCollision(player, delta);
        }
        else if (result?.GetCollider() is TileMapLayer)
        {
            HandleWallCollision();
        }
        
        // Force Y position to remain constant - cannot be pushed vertically
        this.GlobalPosition = new Vector2(this.GlobalPosition.X, lockedY);
    }

    private void HandleKeenCollision(Keen player, double delta)
    {
        if (isPassingThrough)
        {
            return;
        }

        // Check for deadlock (two bots pushing from opposite sides)
        if (player.IsBeingPushedOppositeDirection(direction))
        {
            StartPassThrough();
            return;
        }

        // Test if Keen can be pushed forward
        if (CanPushKeen(player, direction))
        {
            player.Shove(ShovePower * direction, (float)delta);
        }
        else
        {
            // Keen is blocked - pass through
            StartPassThrough();
        }
    }

    private void HandleWallCollision()
    {
        direction *= -1;
        animatedSprite2D.Play("turn");
        
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
        passThroughEndFrame = Engine.GetPhysicsFrames() + PassThroughFrames;
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
