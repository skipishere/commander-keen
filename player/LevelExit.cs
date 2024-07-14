using Godot;
using System;
using System.Diagnostics;

public partial class LevelExit : Area2D
{
	private bool inRange = false;
	private AudioStreamPlayer audioStreamPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		audioStreamPlayer.Finished += OnAudioStreamPlayerFinished;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (inRange && !audioStreamPlayer.Playing)
		{
			this.SetDeferred("monitoring", false);
			
			inRange = false;
			Debug.Print($"Current scene name: {GetTree().CurrentScene.SceneFilePath}");
			game_stats.Levels[GetTree().CurrentScene.SceneFilePath] = true;
			audioStreamPlayer.Play();
		}
	}

    private void OnAudioStreamPlayerFinished()
    {
        GetTree().ChangeSceneToFile("res://scenes/ck1-overworld.tscn");
    }

    public void OnBodyEntered(Node2D body)
	{
		Debug.Print("Level Exit reached");
		if (body is keen)
		{
			inRange = true;
		}
	}
}
