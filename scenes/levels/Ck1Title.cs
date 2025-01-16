using Godot;
using System;

public partial class Ck1Title : Node2D
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void NewGame()
	{
		GetTree().ChangeSceneToFile("res://main.tscn");
	}

	public void QuitGame()
	{
		GetTree().Quit();
	}
}
