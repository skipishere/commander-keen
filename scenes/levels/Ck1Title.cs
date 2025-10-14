using Godot;
using System;

public partial class Ck1Title : Node2D
{
    [Export]
    public Control DefaultButton;

    public override void _Ready()
    {
        DefaultButton.GrabFocus();
        Input.MouseMode = Input.MouseModeEnum.Visible;
    }

    public override void _Process(double delta)
    {
    }

    public void NewGame()
    {
        game_stats.Reset();
        PlayGame();
    }

    public void Continue()
    {
        // Duplicate loading code from GameLogic.cs, todo deduplicate.
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
        PlayGame();
    }

    public void QuitGame()
    {
        GetTree().Quit();
    }

    private void PlayGame()
    {
        GetTree().ChangeSceneToFile("res://main.tscn");
        Input.MouseMode = Input.MouseModeEnum.Hidden;
    }
}
