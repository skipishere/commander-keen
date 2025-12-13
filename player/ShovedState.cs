using Godot;

public partial class ShovedState : State
{
    private float shoveStartX;
    private const float TileSize = 16f;

    public override StateMachine.KeenStates StateType => StateMachine.KeenStates.Shoved;

    public override void PhysicsProcess(double delta, float lastMovementX)
    {
        var distanceTraveled = Mathf.Abs(Character.GlobalPosition.X - shoveStartX);
        // TODO play around with the /2 logic, it should be 1x tile on ground but this feels better,
        // more testing is needed, once the Butler Bots are no longer pushable by Keen.
        var targetDistance = Character.IsOnFloor() ? TileSize / 2: TileSize * 2;
        
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
