using Godot;
using System;
using System.Diagnostics;

public partial class teleporter : Area2D
{
	[Export]
	public Area2D Target;
	private AnimatedSprite2D animatedSprite;
	private Timer timer;
	private OverworldKeen player;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animatedSprite = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		timer = GetNode<Timer>("Timer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (player != null && Input.IsActionJustPressed("move_jump"))
		{
			animatedSprite.Play("transport");
			Debug.Print("Teleporting player");
			timer.Start();
			player.Visible = false;
		}
	}

	public void MovePlayer()
	{
		animatedSprite.Play("idle");
		player.Position = Target.Position;
		player.Visible = true;
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
