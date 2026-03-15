using Godot;

public partial class Main : Node
{
    private SignalManager signalManager;
    private CanvasLayer pauseMenu;
    private UiPanel uiPanel;

    public override void _Ready()
    {
        // Log version information on startup
        VersionInfo.LogVersionInfo();

        pauseMenu = GetNode<CanvasLayer>("Pause Menu");
        uiPanel = GetNode<UiPanel>("UI/UiPanel");

        signalManager = GetNode<SignalManager>("/root/SignalManager");
        signalManager.ExitingLevel += OnExitingLevel;
        signalManager.EnteringLevel += OnEnteringLevel;
        signalManager.GameOver += OnGameOver;

        signalManager.EmitSignal(nameof(SignalManager.ResetUi));
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        signalManager.ExitingLevel -= OnExitingLevel;
        signalManager.EnteringLevel -= OnEnteringLevel;
        signalManager.GameOver -= OnGameOver;
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
        var level = GetNodeOrNull<Node2D>("Level");

        if (game_stats.GameOver)
        {
            GameOver();
        }
        else
        {
            this.AddChild(ResourceLoader.Load<PackedScene>("res://scenes/levels/ck1-overworld.tscn").Instantiate());
        }

        level?.QueueFree();
    }

    private void GameOver()
    {
        var screenshot = this.GetViewport().GetTexture().GetImage();
        var gameOverScreen = ResourceLoader.Load<PackedScene>("res://ui/GameOver.tscn").Instantiate() as GameOver;
        gameOverScreen.FinalScreenshot = screenshot;
        uiPanel.RestoreUiVisibility();

        this.AddChild(gameOverScreen);
    }

    private void OnGameOver()
    {
        uiPanel.OverrideUiVisibility(false);
    }
}
