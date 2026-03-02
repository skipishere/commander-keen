using Godot;
using System;
using System.Diagnostics;

public partial class TankRobot : StaticBody2D
{
    private const float Speed = 0.842f;
    private const float ShovePower = 3.0f;
    private const int KeenCollisionLayer = 2;
    private const float PassThroughDistance = 25f;
    private int direction = 1;
    private AnimationPlayer animationPlayer;
    private RayCast2D rayCastLeft;
    private RayCast2D rayCastRight;
    private bool isPassingThrough;
    private float passThroughStartX;
    private ulong lastTurnFrame;

    public override void _Ready()
    {
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        rayCastLeft = GetNode<RayCast2D>("RayCastLeft");
        rayCastRight = GetNode<RayCast2D>("RayCastRight");
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!rayCastRight.IsColliding() && !rayCastLeft.IsColliding())
        {
            direction *= -1;
            animationPlayer.Play("turn");
            lastTurnFrame = Engine.GetPhysicsFrames();

            if (isPassingThrough)
            {
                EndPassThrough();
            }
        }
        else if (!animationPlayer.IsPlaying() || animationPlayer.CurrentAnimation == "turn")
        {
            animationPlayer.Play(direction > 0 ? "right" : "left");
        }

        var lockedY = this.GlobalPosition.Y;
        var currentX = this.GlobalPosition.X;
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
                var colliderObject = InstanceFromId(colliderId);

                if (colliderObject is Keen player)
                {
                    HandleKeenCollision(player, delta);
                }
                else if (colliderObject is TileMapLayer)
                {
                    Debug.WriteLine($"Collided with {colliderObject}");
                    // Only process wall collision if not recently turned (prevent rapid turn loop)
                    // TODO - This is a bit of a hack and shouldn't be required
                    if (Engine.GetPhysicsFrames() - lastTurnFrame >= 10)
                    {
                        HandleWallCollision();
                    }
                }
            }
        }

        if (isPassingThrough)
        {
            var distanceMoved = Mathf.Abs(GlobalPosition.X - passThroughStartX);
            if (distanceMoved >= PassThroughDistance)
            {
                EndPassThrough();
            }
        }

        this.GlobalPosition = new Vector2(currentX + velocity.X, lockedY);
    }

    private void HandleKeenCollision(Keen player, double delta)
    {
        if (isPassingThrough)
        {
            return;
        }

        if (CanPushKeen(player, direction))
        {
            player.Shove(ShovePower * direction);
        }
        else
        {
            StartPassThrough();
        }
    }

    private void HandleWallCollision()
    {
        direction *= -1;
        animationPlayer.Play("turn");
        lastTurnFrame = Engine.GetPhysicsFrames();

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
}
