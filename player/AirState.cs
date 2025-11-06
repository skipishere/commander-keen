using Godot;

public partial class AirState : State
{
    public override StateMachine.KeenStates StateType => StateMachine.KeenStates.Air;

    private float fallSpeed;

    public override void PhysicsProcess(double delta, float lastMovementX)
    {
        if (Input.IsActionJustReleased("move_jump") && Character.Velocity.Y < 0)
        {
            Character.Velocity = Character.Velocity with { Y = 0 };
        }

        if (Character.IsOnFloor())
        {
            var intensity = Mathf.Clamp(fallSpeed / 800.0f, 0.1f, 1f);
            VibrationManager.TryStartVibration(intensity * 0.5f, intensity, 0.2f);
            NextState = StateMachine.KeenStates.Ground;
            return;
        }

        AnimationTree.Set("parameters/Fall/blend_position", lastMovementX);
        AnimationTree.Set("parameters/Jump/blend_position", lastMovementX);

        var movement = Input.GetAxis("move_left", "move_right");
        Character.Velocity = new Vector2(
            movement * Speed,
            Character.Velocity.Y + gravity * (float)delta);

        fallSpeed = Mathf.Abs(Character.Velocity.Y);
    }

    public override void Enter()
    {
        fallSpeed = 0f;

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
