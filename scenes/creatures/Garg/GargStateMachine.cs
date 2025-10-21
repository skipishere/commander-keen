using Godot;
using System.Collections.Generic;
using System.Linq;

public partial class GargStateMachine : Node
{
    public enum GargStates
    {
        Walk,
        Agro,
        Thinking,
        Dead,
        TurnAround
    }

    private readonly Dictionary<GargStates, IState<GargStates>> states = new();

    private IState<GargStates> Current { get; set; }

    [Export]
    private Garg character;

    [Export]
    private AnimationTree animationTree;

    private CharacterBody2D player;

    public override void _Ready()
    {
        foreach (GargBaseState state in GetChildren().OfType<GargBaseState>())
        {
            state.Character = character;
            state.AnimationTree = animationTree;
            states.Add(state.StateType, state);
        }

        Current = states.First().Value;
    }

    public void PhysicsProcess(double delta, float lastMovementX, bool isActivated)
    {
        if (!isActivated
            && Current.StateType != GargStates.Agro)
        {
            return;
        }

        Current.PhysicsProcess(delta, lastMovementX);

        if (Current.NextState.HasValue)
        {
            ChangeState(Current.NextState.Value);
        }
    }

    private void ChangeState(GargStates newState)
    {
        if (Current.StateType == newState)
        {
            return;
        }

        Current.Exit();
        Current = states[newState];
        Current.Enter();
    }

    public void TakeDamage()
    {
        CallDeferred(nameof(ChangeState), (int)GargStates.Dead);
    }
}
