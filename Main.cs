using System.Diagnostics;
using Godot;

public partial class Main : Node
{
	private SignalManager signalManager;
	private CanvasLayer pauseMenu;

	[Export]
	private Button saveButton;
	
	[Export]
	public Control DefaultButton;

	public override void _Ready()
	{
		pauseMenu = GetNode<CanvasLayer>("Pause Menu");
		signalManager = GetNode<SignalManager>("/root/SignalManager");
		signalManager.ExitingLevel += OnExitingLevel;
		signalManager.EnteringLevel += OnEnteringLevel;

		signalManager.EmitSignal(nameof(SignalManager.ResetUi));
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("ui_cancel"))
		{
			saveButton.Visible = GetNodeOrNull<Node2D>("Map") != null;

			GetTree().Paused = !GetTree().Paused;
			pauseMenu.Visible = GetTree().Paused;
			DefaultButton.GrabFocus();
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

	public void SaveFile()
    {
        var file = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.WriteRead);

        var savedate = Json.Stringify(game_stats.Save());
		Debug.WriteLine(savedate);
        file.StoreLine(savedate);
		file.Close();
    }

    public void LoadFile()
    {
        if (!FileAccess.FileExists("user://savegame.save"))
        {
            return;
        }

		Debug.WriteLine("File found - attempting to load");
        
        var file = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Read);
		var savedata = file.GetAsText();
		var json = new Json();
		var parseResult = json.Parse(savedata);
		if (parseResult != Error.Ok)
		{
			Debug.WriteLine("Error parsing save file");
			return;
		}
        var nodeData = new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)json.Data);
        game_stats.Load(nodeData);
		
		// TODO sort out the UI reset
		signalManager.EmitSignal(nameof(SignalManager.ResetUi));
		
		Resume();
    }

	public void Resume()
	{
		pauseMenu.Visible = false;
		GetTree().Paused = false;
		Debug.WriteLine("Resuming game");
	}
}
