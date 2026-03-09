using Godot;

public abstract partial class TankRobotBaseState : Node, IState<TankRobotStateMachine.TankRobotStates, TankRobot>
{
    public TankRobot Character { get; set; }

    public AnimationTree AnimationTree { get; set; }

    public abstract TankRobotStateMachine.TankRobotStates StateType { get; }

    public virtual bool CanMove => true;
    public TankRobotStateMachine.TankRobotStates? NextState { get; set; }

    internal AnimationNodeStateMachinePlayback playback { get => (AnimationNodeStateMachinePlayback)AnimationTree.Get("parameters/playback"); }

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