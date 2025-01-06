using Godot;
using System;
using System.Diagnostics;
using System.Linq;

public partial class ThinkingState : GargBaseState
{
	public override GargStateMachine.GargStates StateType => GargStateMachine.GargStates.Thinking;

	private CharacterBody2D playerKeen;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		playerKeen = GetTree().GetNodesInGroup("Player").OfType<CharacterBody2D>().First();
	}

	public override void StateInput(InputEvent inputEvent)
	{
	}

	public override void PhysicsProcess(double delta, float lastMovementX)
	{
		// Decide which direction keen is in and walk
		// if in line of sight -> agro run
	}

	public void ThinkingFinished()
	{
		// If keen in line of sight -> agro
		// else walk
		Debug.WriteLine("ThinkingFinished");
		NextState = GargStateMachine.GargStates.Walk;
	}

	public override void Enter()
	{
		Debug.WriteLine("GargThinkingState Enter");
		playback.Travel("Thinking");
	}
}
