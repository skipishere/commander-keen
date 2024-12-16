using Godot;

public partial class LevelExit : Area2D
{
	private AnimationPlayer animationPlayer;
	private SignalManager signalManager;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
		signalManager = GetNode<SignalManager>("/root/SignalManager");
	}

	private void ExitStart()
	{
		var exitPlayer = GetTree().CurrentScene.GetNode<AnimationPlayer>("/root/Main/Level/AnimationPlayer");
		exitPlayer?.Play("level_exit");
	}

    private void ExitFinished()
    {
		signalManager.EmitSignal(nameof(SignalManager.ExitingLevel));
    }

    public void OnBodyEntered(Node2D body)
	{
		if (body is keen player)
		{
			animationPlayer.Play("exiting");
			player.Hide();
			game_stats.Levels[game_stats.CurrentLevel] = true;
		}
	}
}
