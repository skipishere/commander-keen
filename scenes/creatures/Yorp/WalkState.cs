using System;
using Godot;

namespace CommanderKeen.Scenes.Creatures.Yorp;
public partial class WalkState : YorpBaseState
{
    [Export]
    public float MaxJumpVelocity = -200.0f;

    public override YorpStateMachine.YorpStates StateType => YorpStateMachine.YorpStates.Walk;

    private Timer walkTimer;
    private Timer jumpTimer;
    private bool jump = false;

    public override void _Ready()
    {
        walkTimer = GetNode<Timer>("WalkTimer");
        jumpTimer = GetNode<Timer>("JumpTimer");
    }

    private void OnTimerTimeout()
    {
        walkTimer.Stop();
        jumpTimer.Stop();
    }

    public override void StateInput(InputEvent inputEvent)
    {
    }

    public override void PhysicsProcess(double delta, float lastMovementX)
    {
        if (Character.IsOnFloor() && jump)
        {
            jump = false;
            Character.Velocity = Character.Velocity with { Y = random.RandiRange((int)MaxJumpVelocity, -1) };
        }

        if (walkTimer.IsStopped() && Character.IsOnFloor())
        {
            this.NextState = YorpStateMachine.YorpStates.Thinking;
        }

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
        walkTimer.WaitTime = random.RandiRange(1, 5);
        walkTimer.Start();

        jumpTimer.WaitTime = random.RandiRange(1, 4);
        jumpTimer.Start();

        // Look up Keen direction
        var keen = GetTree().GetNodesInGroup("Player")[0] as Keen;
        var direction = keen.GlobalPosition.X < Character.GlobalPosition.X ? Vector2.Left.X : Vector2.Right.X;
        AnimationTree.Set("parameters/Walk/blend_position", direction);
        Character.Velocity = Character.Velocity with { X = direction };
    }

    public override void ExitState()
    {
        walkTimer.Stop();
        jumpTimer.Stop();
    }

    private void Jump()
    {
        jump = true;
    }
}
