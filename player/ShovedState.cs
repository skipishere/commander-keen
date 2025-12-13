using Godot;

public partial class ShovedState : State
{
    private const float JumpVelocity = -315.0f;
    private float shoveStartX;
    private const float TileSize = 16f;

    public override StateMachine.KeenStates StateType => StateMachine.KeenStates.Shoved;

    public override void PhysicsProcess(double delta, float lastMovementX)
    {
        var distanceTraveled = Mathf.Abs(Character.GlobalPosition.X - shoveStartX);
        // TODO play around with the /2 logic, it should be 1x tile on ground but this feels better,
        // more testing is needed, once the Butler Bots are no longer pushable by Keen.
        var targetDistance = Character.IsOnFloor() ? TileSize / 2 : TileSize * 2;
        if (Input.IsActionJustPressed("move_jump") && Character.IsOnFloor())
        {
            Character.Velocity = Character.Velocity with { Y = JumpVelocity };
            VibrationManager.TryStartVibration(0.3f, 0.2f, 0.08f);
            NextState = StateMachine.KeenStates.Air;
            return;
        }

        if (Character.IsOnWall() || distanceTraveled >= targetDistance)
        {
            Character.Velocity = Character.Velocity with { X = 0 };
            NextState = Character.IsOnFloor() ? StateMachine.KeenStates.Ground : StateMachine.KeenStates.Air;
            return;
        }

        Character.Velocity = new Vector2(
            Mathf.Sign(lastMovementX) * Speed,
            Character.Velocity.Y + gravity * (float)delta
        );
    }

    public override void Enter()
    {
        playback.Travel("Walk");
        shoveStartX = Character.GlobalPosition.X;
    }
}
