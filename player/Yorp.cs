using Godot;
using System;
using System.Diagnostics;

public partial class Yorp : CharacterBody2D
{
	private AnimationPlayer animationPlayer;
	private Timer knockedOutTimer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		knockedOutTimer = GetNode<Timer>("KnockedOutTimer");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void KnockedOut(Node body)
	{
		Debug.Print("Yorp eye is sore");
		if (body is keen keen)
		{
			Debug.Print("Yorp knocked out");
			animationPlayer.Play("dazed");
			knockedOutTimer.Start();
		}
	}

	private void KnockedOutTimeout()
	{
		animationPlayer.Play("RESET");
		Debug.Print("Yorp ok");
	}
}
