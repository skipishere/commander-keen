using Godot;
using System;

public partial class Ck1Title : Node2D
{
	[Export]
	public Control DefaultButton;
	
	public override void _Ready()
	{
		DefaultButton.GrabFocus();
	}
	
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
