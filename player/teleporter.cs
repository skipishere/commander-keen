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
	private OverworldKeen player;
	private bool isTeleporting = false;

	[Signal]
	public delegate void TeleportStartEventHandler();

	[Signal]
	public delegate void TeleportCompleteEventHandler();
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		audioStreamPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		timer = GetNode<Timer>("Timer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (player != null && Input.IsActionJustPressed("move_jump"))
		{
			isTeleporting = true;
			Animate();
		}
	}

	public void Animate()
	{
		EmitSignal(SignalName.TeleportStart);
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
			player.Position = Target.Position;
			isTeleporting = false;
		}
		else
		{
			EmitSignal(SignalName.TeleportComplete);
		}
	}

	public void _on_body_entered(Node2D body)
	{
		if (body is OverworldKeen)
		{
			Debug.Print("In range of teleporter");
			player = body as OverworldKeen;			
		}
	}

	public void _on_body_exited(Node2D body)
	{
		if (body is OverworldKeen)
		{
			Debug.Print("Out of range of teleporter");
			player = null;
		}
	}
}
