using Godot;
using System;
using System.Collections.Generic;

public partial class YorpStatue : Node2D
{
	private SignalManager signalManager;
	private string Title = "Yorp Statue"; // TODO load in title and message from level
	private string Message = "I am the Yorp Statue. I am here to help you. I will give you a hint.";
	private AnimatedSprite2D animatedSprite2D;
	private Area2D area2D;

	private Dictionary<string, (string title, string message)> statueMessages = new()
    {
		{ "res://scenes/debug.tscn", ("Odd text appears on screen:", "Insert something clever here...")},
		{ "res://scenes/levels/ck1lv02.tscn", ("You hear in your mind:", "It is too bad that you cannot read the Standard Galactic Alphabet, Human.") },
		{ "res://scenes/levels/ck1lv06.tscn", ("A message echoes in your head:", "The teleporter in the ice will send you to the dark side of Mars.")},
		{ "res://scenes/levels/ck1lv09.tscn", ("A voice buzzes in your mind:", "There is a hidden city. Look in the dark area of the city to the south.")},
		{ "res://scenes/levels/ck1lv10.tscn", ("You see these words in your head:", "You will need a Raygun in the end, but not to shoot the Vorticon...")},
		{ "res://scenes/levels/ck1lv11.tscn", ("You hear in your mind:", "GAAARRRRRGG!")},
		{ "res://scenes/levels/ck1lv12.tscn", ("A Yorpish whisper says:", "Look for dark, hidden bricks. You can see naught but their upper left corner.")},
		{ "res://scenes/levels/ck1lv15.tscn", ("A Yorpy mind-thought bellows:", "You cannot kill the Vorticon Commander directly.")},
	};

	public override void _Ready()
	{
		signalManager = GetNode<SignalManager>("/root/SignalManager");
		animatedSprite2D = GetNode<AnimatedSprite2D>("AnimatedSprite2D");
		area2D = GetNode<Area2D>("Area2D");
	}

	public void OnBodyEntered(Node2D body)
	{
		if (body is keen)
		{
			var (title, message) = statueMessages[game_stats.CurrentLevel];
			
			animatedSprite2D.Stop();
			signalManager.EmitSignal(nameof(SignalManager.ShowUi), title, message);
			area2D.QueueFree();
		}
	}
}
