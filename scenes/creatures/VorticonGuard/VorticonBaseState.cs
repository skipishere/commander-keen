using Godot;

namespace CommanderKeen.Scenes.Creatures.Vorticon;
public abstract partial class VorticonBaseState : Node, IState<VorticonStateMachine.VorticonStates>
{
    internal const float Speed = 45.0f;
    public CharacterBody2D Character { get; set; }

    public AnimationTree AnimationTree { get; set; }

    public abstract VorticonStateMachine.VorticonStates StateType { get; }

    public virtual bool CanMove => true;
    public VorticonStateMachine.VorticonStates? NextState { get; set; }

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