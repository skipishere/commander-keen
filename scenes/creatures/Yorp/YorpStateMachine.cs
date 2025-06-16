using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace CommanderKeen.Scenes.Creatures.Yorp;
public partial class YorpStateMachine : Node
{
    public enum YorpStates
    {
        Walk,
        Thinking,
        Dazed,
        Dead
    }

    private readonly Dictionary<YorpStates, IState<YorpStates>> states = [];

    private IState<YorpStates> Current { get; set; }

    [Export]
    private Yorp character;

    [Export]
    private AnimationTree animationTree;

    private CharacterBody2D player;

    public override void _Ready()
    {
        foreach (YorpBaseState state in GetChildren().OfType<YorpBaseState>())
        {
            state.Character = character;
            state.AnimationTree = animationTree;
            states.Add(state.StateType, state);
            Debug.Print("Yorp Added state: " + state.StateType);
        }

        Current = states.First().Value;
        Debug.Print("Yorp Default state: " + Current.StateType);
        Current.Enter();
    }

    public void PhysicsProcess(double delta, float lastMovementX, bool isActivated)
    {
        if (!isActivated)
        {
            return;
        }

        Current.PhysicsProcess(delta, lastMovementX);

        if (Current.NextState.HasValue)
        {
            ChangeState(Current.NextState.Value);
        }
    }

    private void ChangeState(YorpStates newState)
    {
        if (Current.StateType == newState)
        {
            return;
        }

        Debug.Print($"Yorp State change - old: {Current.StateType}, New: {Current.NextState}");
        Current.Exit();
        Current = states[newState];
        Current.Enter();
    }

    public void TakeDamage()
    {
        CallDeferred(nameof(ChangeState), (int)YorpStates.Dead);
    }

    public void KnockedOut()
    {
        CallDeferred(nameof(ChangeState), (int)YorpStates.Dazed);
    }
}
