using Godot;

public abstract partial class State : Node, IState<StateMachine.KeenStates>
{
    public const float Speed = 180.0f;
    public CharacterBody2D Character { get; set; }
    internal Vector2 shoveVelocity = Vector2.Zero;
    private float shoveStartX = 0f;
    public AnimationTree AnimationTree { get; set; }
    public abstract StateMachine.KeenStates StateType { get; }
    public virtual bool CanMove => true;
    public StateMachine.KeenStates? NextState { get; set; }

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

    public void Shove(float direction)
    {
        shoveVelocity = new Vector2(Mathf.Sign(direction) * Speed, 0);
    }
}