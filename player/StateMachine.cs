using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class StateMachine : Node
{
    public enum KeenStates
    {
        StartLevel,
        Ground,
        Air,
        Shoot,
        Pogo,
        Dead,
        Iced,
        Hidden,
    }

    private readonly Dictionary<KeenStates, IState<KeenStates>> states = new();

    private IState<KeenStates> Current { get; set; }

    [Export]
    private Keen character;

    [Export]
    private AnimationTree animationTree;

    private KeenStates? forceState;

    private SignalManager signalManager;

    public override void _Ready()
    {
        foreach (State state in GetChildren().OfType<State>())
        {
            state.Character = character;
            state.AnimationTree = animationTree;
            states.Add(state.StateType, state);
            //Debug.Print("Keen Added state: " + state.StateType);
        }

        Current = states.First().Value;
        Debug.Print("Keen Default state: " + Current.StateType);

        signalManager = GetNode<SignalManager>("/root/SignalManager");
        signalManager.HidePlayer += () => forceState = KeenStates.Hidden;
        signalManager.KeenDead += () => forceState = KeenStates.Dead;
        signalManager.KeenFrozen += (bool leftIceBlock) =>
        {
            ((IcedState)states[KeenStates.Iced]).HitByLeftIceChunk = leftIceBlock;
            forceState = KeenStates.Iced;
        };
        signalManager.KeenHitYorpEye += () =>
        {
            if (IsPogoing())
            {
                forceState = KeenStates.Air;
            }
        };
    }

    public void PhysicsProcess(double delta, float lastMovementX)
    {
        if (forceState.HasValue)
        {
            ChangeState(forceState.Value);
            forceState = null;
        }
        else if (Current.NextState.HasValue)
        {
            ChangeState(Current.NextState.Value);
        }

        if (Input.IsActionJustPressed("move_shoot") && CheckIfCanMove())
        {
            ChangeState(KeenStates.Shoot);
        }

        if (Input.IsActionJustPressed("move_pogo")
            && CheckIfCanMove()
            && game_stats.HasPogoStick != game_stats.PogoStickState.No)
        {
            ChangeState(Current.StateType == KeenStates.Pogo ? KeenStates.Air : KeenStates.Pogo);
        }

        Current.PhysicsProcess(delta, lastMovementX);
    }

    private void ChangeState(KeenStates newState)
    {
        if (Current.StateType == newState
            && newState != KeenStates.Iced)
        {
            return;
        }

        Current.Exit();
        Current = states[newState];
        Current.Enter();
    }

    private bool CheckIfCanMove()
    {
        return Current.CanMove;
    }

    public bool IsPogoing()
    {
        return Current.StateType == KeenStates.Pogo;
    }
}
