using Godot;
using System;

public partial class Ck1Title : Node2D
{
	[Export]
	public Control DefaultButton;
	
	public override void _Ready()
	{
		DefaultButton.GrabFocus();
	}
	
	public override void _Process(double delta)
	{
	}

	public void NewGame()
	{
		GetTree().ChangeSceneToFile("res://main.tscn");
	}

	public void Continue()
	{
		var file = FileAccess.Open("user://savegame.save", FileAccess.ModeFlags.Read);
		var savedata = file.GetAsText();
		var json = new Json();
		var parseResult = json.Parse(savedata);
		if (parseResult != Error.Ok)
		{
			return;
		}
        var nodeData = new Godot.Collections.Dictionary<string, Variant>((Godot.Collections.Dictionary)json.Data);
        game_stats.Load(nodeData);

		GetTree().ChangeSceneToFile("res://main.tscn");
	}

	public void QuitGame()
	{
		GetTree().Quit();
	}
}
