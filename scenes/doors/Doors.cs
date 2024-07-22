using Godot;
using System;

public partial class Doors : Node2D
{
	[Export]
    public game_stats.KeyCards Card = game_stats.KeyCards.Yellow;

	//private AudioStreamPlayer2D audioStreamPlayer;
	private AnimationPlayer animationPlayer;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		//audioStreamPlayer = GetNode<AudioStreamPlayer2D>("AudioStreamPlayer2D");
		animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public void OnArea2dBodyEntered(Node body)
	{
		if (body is keen keen)
		{
			if (keen.HasKey(Card))
			{
				this.SetDeferred("monitoring", false);
				//audioStreamPlayer.Play();
				animationPlayer.Play("open");
				
				//var tween = GetTree().CreateTween();
				//sprite2D.DrawTextureRectRegion(sprite2D.Texture, new Rect2(0,0,16,16), new Rect2( 0, 0, 16, 32));
				//tween.Parallel().TweenProperty(this, "position", Position + new Vector2(0, 32), .5f);
				//tween.Parallel().TweenProperty(sprite2D, "modulate:a", 0, .5f);
				//tween.TweenCallback(Callable.From(QueueFree));
			}
		}
	}

	public void RemoveDoor()
	{
		this.QueueFree();
	}
}
