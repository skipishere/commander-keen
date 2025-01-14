using Godot;
using System.Diagnostics;

public partial class LevelTeleporterExit : Area2D
{
	private bool inRange = false;
	private AudioStreamPlayer audioStreamPlayer;
	private SignalManager signalManager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		audioStreamPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		audioStreamPlayer.Finished += OnAudioStreamPlayerFinished;
		signalManager = GetNode<SignalManager>("/root/SignalManager");
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
		signalManager.EmitSignal(nameof(SignalManager.ExitingLevel));
    }

    public void OnBodyEntered(Node2D body)
	{
		Debug.Print("Teleporter exit reached");
		if (body is Keen)
		{
			inRange = true;
			signalManager.EmitSignal(nameof(SignalManager.HidePlayer));
		}
	}
}
