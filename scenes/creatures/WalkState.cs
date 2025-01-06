using System;
using System.Diagnostics;
using Godot;

public partial class WalkState : GargBaseState
{
	private const float Speed = 90.0f;
    public override GargStateMachine.GargStates StateType => GargStateMachine.GargStates.Walk;

	private Timer timer;

	public override void _Ready()
	{
		timer = GetNode<Timer>("Timer");
	}

    private void OnTimerTimeout()
    {
		Debug.WriteLine("WalkState OnTimerTimeout");
        this.NextState = GargStateMachine.GargStates.Thinking;
    }

    public override void StateInput(InputEvent inputEvent)
	{
	}

    public override void PhysicsProcess(double delta, float lastMovementX)
	{
		AnimationTree.Set("parameters/Walk/blend_position", lastMovementX);
		
		//if (//keen in sight)
		//{
			//NextState = GargStateMachine.GargStates.Agro;
			//return;
		//}

		// If colide with wall then reverse direction
        //var toSpeed = movement * Speed;
        //player.Velocity = player.Velocity with { X = Mathf.MoveToward(player.Velocity.X, toSpeed, Speed)};
	}

	public override void Enter()
	{
		Debug.WriteLine("GargWalkState Enter");
		playback.Travel("Walk");

		// Walk time is around 1-5 seconds
		var random = new Random().Next(1, 5);
		timer.WaitTime = random;
		timer.Start();
	}
}
