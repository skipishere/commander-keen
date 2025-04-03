using System;
using Godot;

namespace CommanderKeen.Scenes.Creatures.Yorp;
public partial class ThinkingState : YorpBaseState
{
	public override YorpStateMachine.YorpStates StateType => YorpStateMachine.YorpStates.Thinking;
	private Random random = new Random();
	private int thinkAmount;
	private int direction = 0;
	
	public override void _Ready()
	{
	}

	public override void StateInput(InputEvent inputEvent)
	{
	}

	public override void PhysicsProcess(double delta, float lastMovementX)
	{
	}

	public void ThinkingFinished()
	{
		thinkAmount--;
		if (thinkAmount <= 0)
		{
			this.NextState = YorpStateMachine.YorpStates.Walk;
		}
		
		direction *=-1;
		AnimationTree.Set("parameters/Look/blend_position", direction);
    }

	public override void Enter()
	{
		thinkAmount = random.Next(1, 5);
		direction = random.Next(0, 2) == 0 ? -1 : 1;
		
		playback.Travel("Look");
		AnimationTree.Set("parameters/Look/blend_position", direction);
		Character.Velocity = Vector2.Zero;
	}
}
