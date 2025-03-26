using System;
using Godot;

namespace CommanderKeen.Scenes.Creatures.Vorticon;
public partial class JumpState : VorticonBaseState
{
    public override VorticonStateMachine.VorticonStates StateType => VorticonStateMachine.VorticonStates.Jump;

	public override void _Ready()
	{
	}

    private void OnTimerTimeout()
    {
        this.NextState = VorticonStateMachine.VorticonStates.Jump;
    }

    public override void StateInput(InputEvent inputEvent)
	{
	}

    public override void PhysicsProcess(double delta, float lastMovementX)
	{
		if (Character.IsOnWall())
		{
			lastMovementX = -lastMovementX;
		}
		else
		{
			lastMovementX = Character.Velocity.X > 0 ? Vector2.Right.X : Vector2.Left.X;
		}

		if (Character.IsOnFloor())
		{
			this.NextState = VorticonStateMachine.VorticonStates.Thinking;
		}

		AnimationTree.Set("parameters/Jump/blend_position", lastMovementX);
		Character.Velocity = new Vector2(lastMovementX * Speed, Character.Velocity.Y + gravity * (float)delta);
	}

	public override void Enter()
	{
		playback.Travel("Jump");

		// Walk time is around 1-5 seconds
		float random = new Random().Next(0, 6);
		if (random == 0)
		{
			random = 0.5f;
		}
			
		var direction = Character.Velocity.X > 0 ? Vector2.Right.X : Vector2.Left.X;
		AnimationTree.Set("parameters/Jump/blend_position", direction);
		Character.Velocity = new Vector2 { X = direction, Y = random * 32 };
	}
}
