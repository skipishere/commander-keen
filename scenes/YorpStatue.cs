using Godot;
using System;

public partial class YorpStatue : Node2D
{
	private SignalManager signalManager;
	private string Title = "Yorp Statue"; // TODO load in title and message from level
	private string Message = "I am the Yorp Statue. I am here to help you. I will give you a hint.";
	private AnimatedSprite2D animatedSprite2D;
	private Area2D area2D;

	public override void _Ready()
	{
		signalManager = GetNode<SignalManager>("/root/SignalManager");
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		area2D = GetNode<Area2D>("Area2D");
	}

	public void OnBodyEntered(Node2D body)
	{
		if (body is keen keen)
		{
			animatedSprite2D.Stop();
			signalManager.EmitSignal(nameof(SignalManager.ShowUi), Title, Message);
			area2D.QueueFree();
		}
	}
}
