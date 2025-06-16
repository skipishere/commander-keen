using Godot;

namespace CommanderKeen.Scenes.Creatures.Yorp;
public partial class DeathState : YorpBaseState
{
    public override YorpStateMachine.YorpStates StateType => YorpStateMachine.YorpStates.Dead;

    public override bool CanMove => false;

    public override void StateInput(InputEvent inputEvent)
    {
    }

    public override void PhysicsProcess(double delta, float lastMovementX)
    {
        Character.Velocity = new Vector2(0, Character.Velocity.Y + gravity * (float)delta);
    }

    public override void Enter()
    {
        playback.Travel("Die");
    }

}
