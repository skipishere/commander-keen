using Godot;
using System;

public partial class LivesUI : HBoxContainer
{
	private SignalManager signalManager;
	private Label lives;

	private AudioStreamPlayer audioPlayer;

	public override void _Ready()
	{
		lives = GetNode<Label>("Value");
		signalManager = GetNode<SignalManager>("/root/SignalManager");
		audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");
		
		signalManager.ResetUi += OnLivesChanged;
		signalManager.KeenDead += OnLivesChanged;
		signalManager.KeenGainedLife += OnKeenGainLife;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void OnKeenGainLife()
	{
		audioPlayer.Play();
		OnLivesChanged();
	}

	private void OnLivesChanged()
    {
        lives.Text = game_stats.Lives.ToString("00");
    }
}
