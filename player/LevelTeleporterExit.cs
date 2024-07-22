using Godot;
using System;
using System.Diagnostics;

public partial class LevelTeleporterExit : Area2D
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
			game_stats.UsedSecretExit = true;
			audioStreamPlayer.Play();
		}
	}

    private void OnAudioStreamPlayerFinished()
    {
        GetTree().ChangeSceneToFile("res://scenes/ck1-overworld.tscn");
    }

    public void OnBodyEntered(Node2D body)
	{
		Debug.Print("Teleporter exit reached");
		if (body is keen keen)
		{
			inRange = true;
			keen.Visible = false;
		}
	}
}
