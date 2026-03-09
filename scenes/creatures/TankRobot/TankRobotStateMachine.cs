using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class TankRobotStateMachine : Node
{
    public enum TankRobotStates
    {
        Move,
        Shoot,
        Thinking
    }

    private readonly Dictionary<TankRobotStates, IState<TankRobotStates, TankRobot>> states = [];

    private IState<TankRobotStates, TankRobot> Current { get; set; }

    [Export]
    private TankRobot character;

    [Export]
    private AnimationTree animationTree;

    private CharacterBody2D player;

    public override void _Ready()
    {
        foreach (TankRobotBaseState state in GetChildren().OfType<TankRobotBaseState>())
        {
            state.Character = character;
            state.AnimationTree = animationTree;
            states.Add(state.StateType, state);
            Debug.WriteLine("TankRobot Added state: " + state.StateType);
        }

        Current = states.First().Value;
        Current.Enter();
    }

    public void PhysicsProcess(double delta, float lastMovementX, bool isActivated)
    {
        Current.PhysicsProcess(delta, lastMovementX);

        if (Current.NextState.HasValue)
        {
            ChangeState(Current.NextState.Value);
        }
    }

    private void ChangeState(TankRobotStates newState)
    {
        if (Current.StateType == newState)
        {
            return;
        }
        Debug.WriteLine($"TankRobot: Changing state from {Current.StateType} to {newState}");
        Current.Exit();
        Current = states[newState];
        Current.Enter();
    }
}
