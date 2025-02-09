using Godot;

public partial class StoneBlock : CharacterBody2D, ITakeDamage
{
	private bool isShot = false;
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	private CharacterBody2D block;
	private TileMapLayer tileMapLayer;
	private AnimationPlayer animation;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		block = GetNode<CharacterBody2D>("StoneBlock");
		tileMapLayer = GetNode<TileMapLayer>("StoneBlock/TileMapLayer");
		animation = GetNode<AnimationPlayer>("AnimationPlayer");
	}

	public override void _Process(double delta)
	{
		if (isShot && !block.IsOnFloor())
		{
			block.Velocity = block.Velocity with { Y = gravity / 16 * (float)delta };
			
			var result = block.MoveAndCollide(block.Velocity);
			if (result?.GetCollider() is vorticon_guard vorticon)
			{
				vorticon.Kill();
			}
		}
	}

    public void TakeDamage()
    {
        isShot = true;
		tileMapLayer.EraseCell(new Vector2I(-1, -3));
		animation.Play("fall");
    }

}
