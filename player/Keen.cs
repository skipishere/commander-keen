using Godot;
using System.Diagnostics;

public partial class Keen : CharacterBody2D, ITakeDamage
{
    public const float Speed = 180.0f;

    public bool IsPogoing => stateMachine.IsPogoing();

    public AnimationTree animationTree;
    private Camera2D camera;
    private int width;
    private int height;

    private game_stats.KeyCards keyCards = 0;

    public float lastMovementX = Vector2.Right.X;

    public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

    private StateMachine stateMachine;
    
    // Shove mechanics
    private const float TileSize = 16f;
    private bool isBeingShoved = false;
    private float shoveStartX = 0f;
    private float shoveDirection = 0f;
    private const float ShoveSpeed = 180f;
    private bool shovedOffLedge = false;
    
    public bool IsBeingShoved => isBeingShoved;

    public override void _Ready()
    {
        camera = GetNode<Camera2D>("Camera2D");
        animationTree = GetNode<AnimationTree>("AnimationTree");
        stateMachine = GetNode<StateMachine>("StateMachine");

        var collisionShape = GetNode<CollisionShape2D>("CollisionShape2D").Shape.GetRect().Size;
        width = (int)collisionShape.X / 2;
        height = (int)collisionShape.Y / 2;
    }

    public override void _Input(InputEvent @event)
    {
        if (@event.IsActionPressed("move_shoot")
            && Input.IsKeyPressed(Key.T)
            && Input.IsKeyPressed(Key.C))
        {
            Debug.WriteLine("Cheat activated");
            Cheat();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        var movement = Input.GetAxis("move_left", "move_right");
        if (this.Velocity.Normalized().X != 0)
        {
            lastMovementX = this.Velocity.X > 0 ? Vector2.Right.X : Vector2.Left.X;
        }
        else if (movement != 0)
        {
            lastMovementX = movement > 0 ? Vector2.Right.X : Vector2.Left.X;
        }

        stateMachine.PhysicsProcess(delta, lastMovementX);
        
        // Handle shove movement after state machine to override its velocity changes
        if (isBeingShoved)
        {
            ProcessShove();
        }
        
        MoveAndSlide();

        // Clamp the player position to the camera limits.
        var xLimit = Mathf.Clamp(this.GlobalPosition.X, camera.LimitLeft + width, camera.LimitRight - width);
        var yLimit = Mathf.Clamp(this.GlobalPosition.Y, camera.LimitTop - height, camera.LimitBottom - height);
        if (this.GlobalPosition.Y <= camera.LimitTop - height && Velocity.Y < 0)
        {
            // This is needed for maps where there is no 'roof' tiles
            Debug.Print($"Hit the camera top limit - {yLimit}");
            Velocity = Velocity with { Y = 0 };
        }

        this.GlobalPosition = this.GlobalPosition with { X = xLimit, Y = yLimit };
        if (this.IsOnCeiling())
        {
            VibrationManager.TryStartVibration(0f, 0.8f, 0.4f);
        }
    }

    public void Shove(float direction, float delta)
    {
        if (isBeingShoved)
        {
            return; // Already being shoved
        }
        
        // Update facing direction
        shoveDirection = Mathf.Sign(direction);
        lastMovementX = shoveDirection;
        shoveStartX = GlobalPosition.X;
        shovedOffLedge = false;
        isBeingShoved = true;
    }
    
    private void ProcessShove()
    {
        float currentX = GlobalPosition.X;
        float distanceTraveled = Mathf.Abs(currentX - shoveStartX);
        
        // Determine target distance: 1 tile on ground, 2 tiles if went airborne
        if (!IsOnFloor() && !shovedOffLedge)
        {
            shovedOffLedge = true; // Mark that we went airborne
        }
        
        float targetDistance = shovedOffLedge ? (TileSize * 2) : TileSize;
        
        // Check if reached target distance or hit a wall
        if (distanceTraveled >= targetDistance || IsOnWall())
        {
            isBeingShoved = false;
            Velocity = Velocity with { X = 0 };
            return;
        }
        
        // Apply velocity in shove direction
        Velocity = Velocity with { X = shoveDirection * ShoveSpeed };
    }

    public void TakeDamage()
    {
        Debug.WriteLine("Keen hit!");
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

    private void Cheat()
    {
        GiveKey(game_stats.KeyCards.Blue);
        GiveKey(game_stats.KeyCards.Green);
        GiveKey(game_stats.KeyCards.Red);
        GiveKey(game_stats.KeyCards.Yellow);

        game_stats.HasPogoStick = game_stats.PogoStickState.Keep;
        game_stats.Charges = 100;
    }
}
