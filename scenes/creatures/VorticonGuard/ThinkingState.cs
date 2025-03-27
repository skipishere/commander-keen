using System;
using Godot;

namespace CommanderKeen.Scenes.Creatures.Vorticon;
public partial class ThinkingState : VorticonBaseState
{
    public override VorticonStateMachine.VorticonStates StateType => VorticonStateMachine.VorticonStates.Thinking;

	private Timer timer;

	public override void _Ready()
	{
		timer = GetNode<Timer>("ThinkTimer");
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
	}

	public override void Enter()
	{
		// Idle time is around 0.5 - 1 second?
		var random = new Random().Next(1, 3);
		timer.WaitTime = random * 0.5;
		timer.Start();

		Character.Velocity = Vector2.Zero;
	}
}
