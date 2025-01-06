using Godot;

public abstract partial class GargBaseState : Node, IState<GargStateMachine.GargStates>
{
    public CharacterBody2D player { get; set; }

    public AnimationTree AnimationTree { get; set; }

    public abstract GargStateMachine.GargStates StateType { get; }

    public virtual bool CanMove => true;
    public GargStateMachine.GargStates? NextState { get; set; }

    internal AnimationNodeStateMachinePlayback playback { get => (AnimationNodeStateMachinePlayback)AnimationTree.Get("parameters/playback"); }

    internal float gravity = ProjectSettings.GetSetting("physics/2d/default_gravity").AsSingle();

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
    }

}