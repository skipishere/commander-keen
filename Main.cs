using Godot;

public partial class Main : Node
{
    private SignalManager signalManager;
    private CanvasLayer pauseMenu;

    public override void _Ready()
    {
        // Log version information on startup
        VersionInfo.LogVersionInfo();

        pauseMenu = GetNode<CanvasLayer>("Pause Menu");
        signalManager = GetNode<SignalManager>("/root/SignalManager");
        signalManager.ExitingLevel += OnExitingLevel;
        signalManager.EnteringLevel += OnEnteringLevel;

        signalManager.EmitSignal(nameof(SignalManager.ResetUi));
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        signalManager.ExitingLevel -= OnExitingLevel;
        signalManager.EnteringLevel -= OnEnteringLevel;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("ui_cancel") && !game_stats.DialogShowing)
        {
            signalManager.EmitSignal(nameof(SignalManager.PauseMenu), !GetTree().Paused, GetNodeOrNull<Node2D>("Map") != null);
        }
    }

    private void OnEnteringLevel(string levelResource)
    {
        game_stats.CurrentLevel = levelResource;
        var map = GetNode<Node2D>("Map");
        this.AddChild(ResourceLoader.Load<PackedScene>(levelResource).Instantiate());
        map.QueueFree();
    }

    private void OnExitingLevel()
    {
        var level = GetNode<Node2D>("Level");
        this.AddChild(ResourceLoader.Load<PackedScene>("res://scenes/levels/ck1-overworld.tscn").Instantiate());
        level.QueueFree();
    }
}
