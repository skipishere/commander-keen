using System;
using Godot;

namespace CommanderKeen.Scenes.Creatures.Yorp;
public partial class WalkState : YorpBaseState
{
	[Export]
	public float MaxJumpVelocity = -200.0f;
	private Random random = new Random();

    public override YorpStateMachine.YorpStates StateType => YorpStateMachine.YorpStates.Walk;

	private Timer walkTimer;
	private Timer jumpTimer;

	public override void _Ready()
	{
		walkTimer = GetNode<Timer>("WalkTimer");
		jumpTimer = GetNode<Timer>("JumpTimer");
	}

    private void OnTimerTimeout()
    {
        this.NextState = YorpStateMachine.YorpStates.Thinking;
		walkTimer.Stop();
		jumpTimer.Stop();
    }

    public override void StateInput(InputEvent inputEvent)
	{
	}

    public override void PhysicsProcess(double delta, float lastMovementX)
	{
		if (Character.IsOnFloor() && Character.IsOnWall())
		{
			lastMovementX = -lastMovementX;
		}
		else
		{
			lastMovementX = Character.Velocity.X > 0 ? Vector2.Right.X : Vector2.Left.X;
		}

		AnimationTree.Set("parameters/Walk/blend_position", lastMovementX);
		Character.Velocity = new Vector2(lastMovementX * Speed, Character.Velocity.Y + gravity * (float)delta);
	}

	public override void Enter()
	{
		playback.Travel("Walk");

		// Walk time is around 1-5 seconds
		walkTimer.WaitTime = random.Next(1, 5);
		walkTimer.Start();

		jumpTimer.WaitTime = random.Next(1, 5);
		jumpTimer.Start();

		// Look up Keen direction
		var keen = GetTree().GetNodesInGroup("Player")[0] as Keen;
		var direction = keen.GlobalPosition.X < Character.GlobalPosition.X ? Vector2.Left.X : Vector2.Right.X;
		AnimationTree.Set("parameters/Walk/blend_position", direction);
		Character.Velocity = Character.Velocity with { X = direction };
	}

	private void Jump()
	{
		if (Character.IsOnFloor())
		{
			Character.Velocity = new Vector2(Character.Velocity.X, random.Next((int)MaxJumpVelocity, 0));
		}
	}
}
