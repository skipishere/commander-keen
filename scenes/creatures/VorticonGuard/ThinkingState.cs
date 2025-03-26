using System;
using Godot;

namespace CommanderKeen.Scenes.Creatures.Vorticon;
public partial class ThinkingState : VorticonBaseState
{
    public override VorticonStateMachine.VorticonStates StateType => VorticonStateMachine.VorticonStates.Thinking;

	private Timer timer;

	public override void _Ready()
	{
		timer = GetNode<Timer>("Timer");
	}

    private void OnTimerTimeout()
    {
        this.NextState = VorticonStateMachine.VorticonStates.Walk;
    }

    public override void StateInput(InputEvent inputEvent)
	{
	}

    public override void PhysicsProcess(double delta, float lastMovementX)
	{
		var random = new Random().Next(-1, 1);
		AnimationTree.Set("parameters/Idle/blend_position", random);
	}

	public override void Enter()
	{
		playback.Travel("Idle");

		// Idle time is around 1-3 seconds?
		var random = new Random().Next(1, 3);
		timer.WaitTime = random;
		timer.Start();

		Character.Velocity = Vector2.Zero;
	}
}
