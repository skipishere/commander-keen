using System.Diagnostics;
using Godot;

public partial class LevelPortal : Area2D
{
    [Export]
    public PackedScene Target;

    private bool inRange = false;
    private SignalManager signalManager;
    private Node2D doneMessage;

    public override void _Ready()
    {
        signalManager = GetNode<SignalManager>("/root/SignalManager");
        signalManager.ResetUi += UpdateLevelStatus;
        UpdateLevelStatus();
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        signalManager.ResetUi -= UpdateLevelStatus;
    }

    private void UpdateLevelStatus()
    {
        doneMessage = GetNode<Node2D>("DoneMessage");

        if (!game_stats.Levels.TryGetValue(this.Target.ResourcePath, out var levelFinished))
        {
            game_stats.Levels.Add(this.Target.ResourcePath, false);
        }

        doneMessage.Visible = levelFinished;
        this.Monitoring = !levelFinished;

        if (levelFinished && this.GetNodeOrNull<StaticBody2D>("StaticBody2D") != null)
        {
            this.GetNode<StaticBody2D>("StaticBody2D").QueueFree();
        }
    }

    public override void _PhysicsProcess(double delta)
    {
        if (inRange && Input.IsActionJustReleased("move_jump"))
        {
            signalManager.EmitSignal(nameof(SignalManager.EnteringLevel), this.Target.ResourcePath);
        }
    }

    public void OnBodyEntered(Node2D body)
    {
        if (body is OverworldKeen)
        {
            Debug.Print($"Level Portal activated - {Target.ResourcePath}");
            inRange = true;
        }
    }

    public void OnBodyExited(Node2D body)
    {
        if (body is OverworldKeen)
        {
            inRange = false;
        }
    }
}
