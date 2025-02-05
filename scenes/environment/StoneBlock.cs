using Godot;
using System;

public partial class StoneBlock : CharacterBody2D, ITakeDamage
{
	private bool isShot = false;
	public float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (isShot && !IsOnFloor())
		{
			Velocity = Velocity with { Y = gravity/16 * (float)delta };
			
			var result = MoveAndCollide(Velocity);
			if (result?.GetCollider() is vorticon_guard vorticon)
			{
				vorticon.Kill();
			}
		}
	}

    public void TakeDamage()
    {
        isShot = true;
    }

}
