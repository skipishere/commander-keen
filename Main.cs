using Godot;

public partial class Main : Node
{
	private SignalManager signalManager;
	
	public override void _Ready()
	{
		signalManager = GetNode<SignalManager>("/root/SignalManager");
		signalManager.ExitingLevel += OnExitingLevel;
		signalManager.EnteringLevel += OnEnteringLevel;

		signalManager.EmitSignal(nameof(SignalManager.ResetUi));
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
