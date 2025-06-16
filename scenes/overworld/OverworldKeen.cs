using System.Diagnostics;
using Godot;

public partial class OverworldKeen : CharacterBody2D
{
    [Export]
    public AnimationTree AnimationTree;

    private Vector2 lastDirection = new(0, 1);

    private bool GodModeEnabled = false;
    private bool ignoreKeys = false;

    public const float Speed = 100.0f;

    public static Vector2? mapPosition;

    private Camera2D camera;
    private int width;
    private int height;
    public bool IsTeleporting = false;

    private SignalManager signalManager;

    public override void _Ready()
    {
        camera = GetNode<Camera2D>("Camera2D");

        var collisionShape = GetNode<CollisionShape2D>("CollisionShape2D").Shape.GetRect().Size;
        width = (int)collisionShape.X / 2;
        height = (int)collisionShape.Y;

        signalManager = GetNode<SignalManager>("/root/SignalManager");
        signalManager.TeleportStart += OnTeleportStart;
        signalManager.TeleportComplete += OnTeleportComplete;
        signalManager.EnteringLevel += OnEnteringLevel;
        signalManager.ResetUi += SetKeenPosition;

        SetKeenPosition();
    }

    public override void _ExitTree()
    {
        signalManager.TeleportStart -= OnTeleportStart;
        signalManager.TeleportComplete -= OnTeleportComplete;
        signalManager.EnteringLevel -= OnEnteringLevel;
        signalManager.ResetUi -= SetKeenPosition;
    }

    private void SetKeenPosition()
    {
        Debug.Print($"OverworldKeen has {mapPosition}.");
        if (mapPosition.HasValue)
        {
            Debug.Print($"Saved Coords x {mapPosition.Value.X} y {mapPosition.Value.Y}");
            this.Position = mapPosition.Value;
        }
    }

    public void Remove()
    {
        signalManager.TeleportStart -= OnTeleportStart;
        signalManager.TeleportComplete -= OnTeleportComplete;
        signalManager.EnteringLevel -= OnEnteringLevel;
    }

    private void OnTeleportStart()
    {
        this.IsTeleporting = true;
    }

    private void OnTeleportComplete(Vector2 position, bool finished)
    {
        this.Position = position;
        this.IsTeleporting = !finished;
        SetPhysicsProcess(finished);
    }

    private void OnEnteringLevel(string levelResource)
    {
        mapPosition = Position;
    }

    public override void _PhysicsProcess(double delta)
    {
        if (Input.IsKeyPressed(Key.G)
            && Input.IsKeyPressed(Key.O)
            && Input.IsKeyPressed(Key.D))
        {
            if (!ignoreKeys)
            {
                GodModeEnabled = !GodModeEnabled;
                this.SetCollisionMaskValue(1, !GodModeEnabled);
                Debug.Print($"God mode is {GodModeEnabled}");
                ignoreKeys = true;
            }
        }
        else
        {
            ignoreKeys = false;
        }

        var direction = Input.GetVector("move_left", "move_right", "map_up", "map_down");
        if (direction != Vector2.Zero)
        {
            lastDirection = direction.Normalized();
        }

        Velocity = new Vector2(direction.X * Speed, direction.Y * Speed);
        AnimationTree.Set("parameters/Walk/blend_position", lastDirection);
        AnimationTree.Set("parameters/Idle/blend_position", lastDirection);

        MoveAndSlide();

        // Clamp the player position to the camera limits.
        var xLimit = Mathf.Clamp(this.GlobalPosition.X, camera.LimitLeft + width, camera.LimitRight - width);
        var yLimit = Mathf.Clamp(this.GlobalPosition.Y, camera.LimitTop, camera.LimitBottom - height);

        this.GlobalPosition = this.GlobalPosition with { X = xLimit, Y = yLimit };
        mapPosition = this.Position;
    }
}
