using Godot;

public partial class HiddenState : State
{
    public override StateMachine.KeenStates StateType => StateMachine.KeenStates.Hidden;

    public override bool CanMove => false;

    public override void StateInput(InputEvent inputEvent)
    {
    }

    public override void PhysicsProcess(double delta, float lastMovementX)
    {
        Character.Velocity = Vector2.Zero;
    }

    public override void Enter()
    {
        playback.Travel("Hidden");
    }
}
