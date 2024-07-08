using Godot;
using System;
using System.Diagnostics;

public partial class teleporter : Area2D
{
	[Export]
	public teleporter Target;
	private AnimatedSprite2D animatedSprite;
	private AudioStreamPlayer2D audioStreamPlayer;
	private Timer timer;
	private bool inRange = false;
	private bool isTeleporting = false;
	private SignalManager signalManager;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		audioStreamPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		timer = GetNode<Timer>("Timer");
		signalManager = GetNode<SignalManager>("/root/SignalManager");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (inRange && Input.IsActionJustPressed("move_jump"))
		{
			isTeleporting = true;
			Animate();
		}
	}

	public void Animate()
	{
		signalManager.EmitSignal(nameof(SignalManager.TeleportStart));
		
		audioStreamPlayer.Play();
		animatedSprite.Play("transport");
		timer.Start();
	}

	public void Idle()
	{
		animatedSprite.Play("idle");
		if (isTeleporting)
		{
			Target.Animate();
			isTeleporting = false;
			signalManager.EmitSignal(nameof(SignalManager.TeleportComplete), Target.Position, false);
		}
		else
		{
			signalManager.EmitSignal(nameof(SignalManager.TeleportComplete), this.Position, true);
		}
	}

	public void _on_body_entered(Node2D body)
	{
		if (body is OverworldKeen)
		{
			inRange = true;
		}
	}

	public void _on_body_exited(Node2D body)
	{
		if (body is OverworldKeen)
		{
			inRange = false;
		}
	}
}
