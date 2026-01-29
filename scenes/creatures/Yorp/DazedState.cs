using Godot;

namespace CommanderKeen.Scenes.Creatures.Yorp;

public partial class DazedState : YorpBaseState
{
    public override YorpStateMachine.YorpStates StateType => YorpStateMachine.YorpStates.Dazed;

    private Timer timer;

    public override void _Ready()
    {
        timer = GetNode<Timer>("KnockedOutTimer");
    }

    private void OnTimerTimeout()
    {
        this.NextState = YorpStateMachine.YorpStates.Thinking;
    }

    public override void StateInput(InputEvent inputEvent)
    {
    }

    public override void PhysicsProcess(double delta, float lastMovementX)
    {
        Character.Velocity = new Vector2(0, Character.Velocity.Y + gravity * (float)delta);
    }

    public override void Enter()
    {
        playback.Travel("Dazed");
        timer.Start();
    }

}
