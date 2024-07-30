using Godot;

public partial class Doors : Node2D
{
	[Export]
    public game_stats.KeyCards Card = game_stats.KeyCards.Yellow;

	private AnimationPlayer animationPlayer;

	public override void _Ready()
	{
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public void OnArea2dBodyEntered(Node body)
	{
		if (body is keen keen)
		{
			if (keen.HasKey(Card))
			{
				this.SetDeferred("monitoring", false);
				animationPlayer.Play("open");
			}
		}
	}

	public void RemoveDoor()
	{
		this.QueueFree();
	}
}
