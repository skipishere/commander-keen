using Godot;
using System.Diagnostics;

public partial class Pause : PanelContainer
{
	[Export]
	private Button saveButton;
	
	[Export]
	public Control DefaultButton;

	private SignalManager signalManager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		signalManager = GetNode<SignalManager>("/root/SignalManager");
		signalManager.PauseMenu += OnPauseMenu;
	}

    private void OnPauseMenu(bool paused, bool showSaveButton)
    {
        saveButton.Visible = showSaveButton;
		GetTree().Paused = paused;
		Visible = paused;

		if (paused)
		{
			DefaultButton.GrabFocus();
		}
    }

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
	{
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
		OnPauseMenu(false, false);
	}
}
