using System;
using System.Diagnostics;
using Godot;

public partial class PogoState : State
{
    private const float PogoVelocity = -315.0f;
    private bool bounce = false;

    public override StateMachine.KeenStates StateType => StateMachine.KeenStates.Pogo;

    public override void StateInput(InputEvent inputEvent)
    {
    }

    public override void PhysicsProcess(double delta, float lastMovementX)
    {
        var movement = Input.GetAxis("move_left", "move_right");
        float toSpeed;

        if (movement == 0)
        {
            // With no player input, the player will slow down to 1/2 max speed or slower.
            toSpeed = Mathf.Clamp(Character.Velocity.X, -Speed / 2, Speed / 2);
        }
        else
        {
            // this is a bit more interesting, changing direction can additively slow the player down to 0.
            toSpeed = Mathf.Clamp(Character.Velocity.X + (movement * Speed * 0.05f), -Speed, Speed);
        }

        AnimationTree.Set("parameters/Pogo/blend_position", lastMovementX);
        AnimationTree.Set("parameters/Pogo-Ground/blend_position", lastMovementX);

        if (Character.IsOnFloor())
        {
            playback.Travel("Pogo-Ground");

            if (bounce)
            {
                if (Input.IsActionPressed("move_jump"))
                {
                    Character.Velocity = Character.Velocity with { Y = PogoVelocity * 1.4f };
                }
                else
                {
                    Character.Velocity = Character.Velocity with { Y = PogoVelocity };
                }
                bounce = false;
            }
        }
        else
        {
            playback.Travel("Pogo");
            Character.Velocity = Character.Velocity with { Y = Character.Velocity.Y + gravity * (float)delta };
        }

        Character.Velocity = Character.Velocity with { X = Mathf.MoveToward(Character.Velocity.X, toSpeed, Speed) };
    }

    public override void Enter()
    {
        playback.Travel("Pogo-Ground");
        bounce = false;
    }

    public void BounceDelayTimeout()
    {
        Debug.WriteLine("Bounce delay finished");
        bounce = true;
    }
}
