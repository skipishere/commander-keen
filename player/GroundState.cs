using System;
using System.Diagnostics;
using Godot;

public partial class GroundState : State
{
    public override StateMachine.KeenStates StateType => StateMachine.KeenStates.Ground;

	[Export]
	private RayCast2D rayCast;

	private bool wasIdle = true;
	
	private enum GroundType
	{
		Normal,
		
		// Can change direction but there is a delay before stopping.
		Slippery,
		
		// Can't change direction, keep on moving in given direction unless you jump and change direction.
		Ice,
	}

	public override void StateInput(InputEvent inputEvent)
	{
	}

    public override void PhysicsProcess(double delta, float lastMovementX)
	{
		AnimationTree.Set("parameters/Idle/blend_position", lastMovementX);
		AnimationTree.Set("parameters/Walk/blend_position", lastMovementX);

		if (Input.IsActionJustPressed("move_jump") || !Character.IsOnFloor())
		{
			NextState = StateMachine.KeenStates.Air;
			return;
		}

		var groundType = GetGroundType();
		
		// Get the input direction and handle the movement/deceleration.
		// TODO Statemachine for Ice and Slippery should be used.
		var movement = Input.GetAxis("move_left", "move_right");
		 if (movement != 0 && groundType != GroundType.Slippery)
		 {
		 	if (groundType != GroundType.Ice)
		 	{
				var toSpeed = movement * Speed;
				Character.Velocity = Character.Velocity with { X = Mathf.MoveToward(Character.Velocity.X, toSpeed, Speed)};
			}
		}
		else
		{
			var toSpeed = 0f;
			if (groundType == GroundType.Ice)
			{
				toSpeed = lastMovementX * Speed;
			}
			else if (groundType == GroundType.Slippery)
			{
				// TODO deal with falling from an ice ledge onto ice, as the velocity should not just hit 0.
				if (movement == 0)
				{
					// With no player input, the player will slow down to 1/2 max speed or slower.
					toSpeed = Mathf.Clamp(Character.Velocity.X, -Speed / 2, Speed / 2);
				}
				else
				{
					// this is a bit more interesting, changing direction can additively slow the player down to 0.
					toSpeed = Mathf.Clamp(Character.Velocity.X + (movement * Speed * 0.05f), -Speed, Speed);
				}
			}
			
			Character.Velocity = Character.Velocity with { X = Mathf.MoveToward(Character.Velocity.X, toSpeed, Speed)};
		}

		
		if ((Character.Velocity.X == 0 && !wasIdle) || groundType == GroundType.Ice)
		{
			playback.Travel("Idle");
			wasIdle = true;
		}
		else if (Character.Velocity.X != 0 && wasIdle)
		{
			playback.Travel("Walk");
			wasIdle = false;
		}
	}

	public override void Enter()
	{
		playback.Travel("Idle");
		wasIdle = true;
	}

	private GroundType GetGroundType()
	{
        if (rayCast.GetCollider() is not TileMapLayer tileMapLayer)
        {
			// We're not on the ground, so we'll just go with the default.
            return GroundType.Normal;
        }

        var local_col_point = tileMapLayer.ToLocal(rayCast.GetCollisionPoint());
		var cell_coords = tileMapLayer.LocalToMap(local_col_point);	
		var cellData = tileMapLayer.GetCellTileData(cell_coords);
		
		try
		{
			// We sometimes get null on the line below, possibly due to tilemaplayer issues the camera has had to deal with?
			var slide = cellData.GetCustomData("Slide");
			return slide.ToString() switch
			{
				"1" => GroundType.Slippery,
				"2" => GroundType.Ice,
				_ => GroundType.Normal
			};
		}
		catch (Exception e)
		{
			Debug.WriteLine($"Error: {e.Message}");
			return GroundType.Normal;
		}
	}
}
