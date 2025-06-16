using System;
using Godot;

public partial class WalkState : GargBaseState
{
    private const float Speed = 45.0f;
    public override GargStateMachine.GargStates StateType => GargStateMachine.GargStates.Walk;

    private Timer timer;

    public override void _Ready()
    {
        timer = GetNode<Timer>("Timer");
    }

    private void OnTimerTimeout()
    {
        this.NextState = GargStateMachine.GargStates.Thinking;
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
        var random = new Random().Next(1, 5);
        timer.WaitTime = random;
        timer.Start();

        // Look up Keen direction
        var keen = GetTree().GetNodesInGroup("Player")[0] as Keen;
        var direction = keen.GlobalPosition.X < Character.GlobalPosition.X ? Vector2.Left.X : Vector2.Right.X;
        AnimationTree.Set("parameters/Walk/blend_position", direction);
        Character.Velocity = Character.Velocity with { X = direction };
    }
}
