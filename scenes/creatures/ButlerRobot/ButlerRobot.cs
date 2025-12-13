using System.Diagnostics;
using Godot;

public partial class ButlerRobot : StaticBody2D, ITakeDamage
{
    private const float Speed = 0.842f;
    private const int KeenCollisionLayer = 2;
    private int direction = 1;
    private AnimatedSprite2D animatedSprite2D;
    private RayCast2D rayCastLeft;
    private RayCast2D rayCastRight;
    private float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();
    private Timer hitTimer;
    private bool isPassingThrough;
    private float passThroughStartX;

    public override void _Ready()
    {
        animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
        rayCastLeft = GetNode<RayCast2D>("RayCastLeft");
        rayCastRight = GetNode<RayCast2D>("RayCastRight");
        hitTimer = GetNode<Timer>("HitTimer");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!rayCastRight.IsColliding() && !rayCastLeft.IsColliding())
        {
            direction *= -1;
            animatedSprite2D.Play("turn");
            EndPassThrough();
        }
        else if (!animatedSprite2D.IsPlaying() || animatedSprite2D.Animation == "turn")
        {
            animatedSprite2D.Play(direction > 0 ? "right" : "left");
        }

        if (isPassingThrough)
        {
            var distanceMoved = Mathf.Abs(GlobalPosition.X - passThroughStartX);
            if (distanceMoved >= 12f)
            {
                EndPassThrough();
            }
        }

        var velocity = new Vector2(direction * Speed, 0);
        
        var testParams = new PhysicsTestMotionParameters2D
        {
            From = this.GlobalTransform,
            Motion = velocity
        };
        var testResult = new PhysicsTestMotionResult2D();
        
        if (PhysicsServer2D.BodyTestMotion(this.GetRid(), testParams, testResult))
        {
            if (testResult.GetCollider() != null)
            {
                var colliderObject = InstanceFromId(testResult.GetColliderId());
                
                if (colliderObject is Keen player)
                {
                    HandleKeenCollision(player, direction);
                }
                else if (colliderObject is TileMapLayer)
                {
                    HandleWallCollision();
                }
            }
        }

        this.GlobalPosition = new Vector2(
            this.GlobalPosition.X + velocity.X,
            this.GlobalPosition.Y);
    }

    private void HandleKeenCollision(Keen keen, int direction)
    {
        if (isPassingThrough)
        {
            return;
        }

        var testParams = new PhysicsTestMotionParameters2D
        {
            From = keen.GlobalTransform,
            Motion = new Vector2(direction * 3f, 0)
        };
        var testResult = new PhysicsTestMotionResult2D();
        var canPush = !PhysicsServer2D.BodyTestMotion(keen.GetRid(), testParams, testResult);

        if (canPush)
        {
            keen.Shove(direction);
        }
        else
        {
            StartPassThrough();
        }
    }

    private void HandleWallCollision()
    {
        direction *= -1;
        animatedSprite2D.Play("turn");
        EndPassThrough();
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
}
