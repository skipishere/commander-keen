using Godot;

public partial class ThinkingState : GargBaseState
{
    public override GargStateMachine.GargStates StateType => GargStateMachine.GargStates.Thinking;

    [Export]
    private RayCast2D left;

    [Export]
    private RayCast2D right;

    [Export]
    private RayCast2D leftCheck;

    [Export]
    private RayCast2D rightCheck;

    public override void _Ready()
    {
    }

    public override void StateInput(InputEvent inputEvent)
    {
    }

    public override void PhysicsProcess(double delta, float lastMovementX)
    {
    }

    public void ThinkingFinished()
    {
        NextState = CanSeeKeen()
        ? GargStateMachine.GargStates.Agro
        : GargStateMachine.GargStates.Walk;
    }

    public override void Enter()
    {
        playback.Travel("Thinking");
        Character.Velocity = Vector2.Zero;
    }

    private bool CanSeeKeen()
    {
        return (left.IsColliding() && leftCheck.IsColliding())
            || (right.IsColliding() && rightCheck.IsColliding());
    }
}
