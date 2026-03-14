using Godot;

public partial class Zap : Node2D
{
    private AnimationPlayer animationPlayer;
    private Sprite2D zapSprite;
    private static readonly RandomNumberGenerator rnd = new();

    public override void _Ready()
    {
        zapSprite = GetNode<Sprite2D>("Zap");
        zapSprite.Frame = rnd.RandiRange(0, 1);
        animationPlayer = GetNode<AnimationPlayer>("AnimationPlayer");
        animationPlayer.Play("Vanish");
    }

    public void Finish()
    {
        QueueFree();
    }
}
