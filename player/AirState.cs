using System.Diagnostics;
using Godot;

public partial class AirState : State
{
    public override StateMachine.KeenStates StateType => StateMachine.KeenStates.Air;

    public override void PhysicsProcess(double delta, float lastMovementX)
    {
        if (Input.IsActionJustReleased("move_jump") && Character.Velocity.Y < 0)
        {
            Character.Velocity = Character.Velocity with { Y = 0 };
        }

        if (Character.IsOnFloor())
        {
            NextState = StateMachine.KeenStates.Ground;
            return;
        }

        AnimationTree.Set("parameters/Fall/blend_position", lastMovementX);
        AnimationTree.Set("parameters/Jump/blend_position", lastMovementX);

        var movement = Input.GetAxis("move_left", "move_right");
        Character.Velocity = new Vector2(
            movement * Speed,
            Character.Velocity.Y + gravity * (float)delta);
    }

    public override void Enter()
    {
        if (Input.IsActionPressed("move_jump") && Character.IsOnFloor())
        {
            playback.Travel("Jump");
        }
        else
        {
            playback.Travel("Fall");
        }
    }
}
