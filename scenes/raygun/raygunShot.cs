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
    private AudioStreamPlayer2D audioStreamPlayer2D;
    private bool hasCollided = false;

    public override void _Ready()
    {
        fireParticles = GetNode<GpuParticles2D>("Fire");
        zap = GetNode<Sprite2D>("Zap");
        collisionShape = GetNode<CollisionShape2D>("CollisionShape2D");
        audioStreamPlayer2D = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer");
    }

    public void SetDirection(Vector2 globalPosition, Vector2 direction, GodotObject origin)
    {
        this.GlobalPosition = globalPosition;
        this.direction = direction;
        this.Transform = this.Transform with { X = this.Transform.X * (direction.X > 0 ? 1 : -1) };
        this.origin = origin;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (hasCollided)
        {
            collisionShape.Disabled = true;
            if (!audioStreamPlayer2D.Playing)
            {
                QueueFree();
            }

            return;
        }

        var modifier = this.zap.Visible ? 0.1f : 1.0f;
        var shot = MoveAndCollide(direction * SPEED * modifier * (float)delta);

        if (shot != null && shot.GetCollider() != origin)
        {
            zap.FlipH = direction.X < 0;
            collisionShape.Disabled = true;
            fireParticles.Emitting = false;

            if (shot.GetCollider() is ITakeDamage creature)
            {
                Debug.Print($"Collided with {creature.GetType()}");
                hasCollided = true;
                creature.TakeDamage();
            }
            else
            {
                Debug.Print("Collided with something that doesn't take damage");

                zap.Visible = true;
                var tween = GetTree().CreateTween();
                tween.Parallel().TweenProperty(this, "scale", new Vector2(0, 0), 0.5f);
                tween.Parallel().TweenProperty(this, "modulate:a", 0, 0.5f);
                tween.TweenCallback(Callable.From(ShotHit));
            }
        }
    }

    public void ShotHit()
    {
        hasCollided = true;
    }

    private void ScreenExited()
    {
        hasCollided = true;
    }
}
