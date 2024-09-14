using Godot;
using System.Diagnostics;

public partial class LevelExit : Area2D
{
	private AnimationPlayer animationPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	private void ExitStart()
	{
		var exitPlayer = GetTree().CurrentScene.GetNode<AnimationPlayer>("AnimationPlayer");
		exitPlayer?.Play("level_exit");
	}

    private void ExitFinished()
    {
		GetTree().ChangeSceneToFile("res://scenes/levels/ck1-overworld.tscn");
    }

    public void OnBodyEntered(Node2D body)
	{
		Debug.Print("Level Exit reached");
		if (body is keen player)
		{
			animationPlayer.Play("exiting");
			player.Hide();
			game_stats.Levels[GetTree().CurrentScene.SceneFilePath] = true;
		}
	}
}
