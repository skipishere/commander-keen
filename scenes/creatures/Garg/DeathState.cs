using System;
using System.Diagnostics;
using Godot;

public partial class DeathState : GargBaseState
{
	public override GargStateMachine.GargStates StateType => GargStateMachine.GargStates.Dead;

	public override bool CanMove => false;

	public override void StateInput(InputEvent inputEvent)
	{
	}

    public override void PhysicsProcess(double delta, float lastMovementX)
	{
	}

	public override void Enter()
	{
		playback.Travel("Die");
		player.Velocity = Vector2.Zero;
	}

}
