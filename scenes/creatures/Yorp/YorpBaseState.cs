using Godot;

namespace CommanderKeen.Scenes.Creatures.Yorp;
public abstract partial class YorpBaseState : Node, IState<YorpStateMachine.YorpStates>
{
    internal const float Speed = 25.0f;
    public CharacterBody2D Character { get; set; }

    public AnimationTree AnimationTree { get; set; }

    public abstract YorpStateMachine.YorpStates StateType { get; }

    public virtual bool CanMove => true;
    public YorpStateMachine.YorpStates? NextState { get; set; }

    internal AnimationNodeStateMachinePlayback playback { get => (AnimationNodeStateMachinePlayback)AnimationTree.Get("parameters/playback"); }

    internal float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle() / 1.5f;

    public virtual void StateInput(InputEvent inputEvent)
    {
    }

    public abstract void PhysicsProcess(double delta, float lastMovementX);

    public virtual void Enter()
    {
    }

    public void Exit()
    {
        NextState = null;
        ExitState();
    }

    public virtual void ExitState()
    {
    }
}