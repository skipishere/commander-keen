using Godot;
using System.Diagnostics;

public partial class raygunShot : StaticBody2D, ITakeDamage
{
    [Export]
    private PackedScene zapScene;
    const int SPEED = 300;
    private Vector2 direction;
    private GpuParticles2D fireParticles;
    private CollisionShape2D collisionShape;
    private GodotObject origin;
    private AudioStreamPlayer2D audioStreamPlayer2D;
    private bool hasCollided = false;

    public Color ShotColor = Colors.White;

    public override void _Ready()
    {
        fireParticles = GetNode<GpuParticles2D>("Fire");
        fireParticles.Modulate = ShotColor;
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

        var shot = MoveAndCollide(direction * SPEED * (float)delta);

        if (shot != null && shot.GetCollider() != origin)
        {
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
                ShowZap();
            }
        }
    }

    private void ScreenExited()
    {
        hasCollided = true;
    }

    public void TakeDamage()
    {
        ShowZap();
    }

    private void ShowZap()
    {
        var zapInstance = zapScene.Instantiate() as Zap;
        zapInstance.GlobalPosition = this.GlobalPosition;

        hasCollided = true;
        GetTree().Root.AddChild(zapInstance);
    }
}
