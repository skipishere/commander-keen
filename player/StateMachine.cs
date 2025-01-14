using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class StateMachine : Node
{
	public enum KeenStates
	{
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
			Debug.Print("Added state: " + state.StateType);
		}
		
		Current = states.First().Value;
		Debug.Print("Default state: " + Current.StateType);

		signalManager = GetNode<SignalManager>("/root/SignalManager");
		signalManager.HidePlayer += () => forceState = KeenStates.Hidden;
		signalManager.KeenDead += () => forceState = KeenStates.Dead;
	}

    public void PhysicsProcess(double delta, float lastMovementX)
	{
		Current.PhysicsProcess(delta, lastMovementX);

		if (forceState.HasValue)
		{
			Debug.Print($"Forced State change - old: {Current.StateType}, New: {forceState}");
			ChangeState(forceState.Value);
			forceState = null;
		} 
		else if (Current.NextState.HasValue)
		{
			Debug.Print($"State change - old: {Current.StateType}, New: {Current.NextState}");
			ChangeState(Current.NextState.Value);
		}
		
		if (Input.IsActionJustPressed("move_shoot") && CheckIfCanMove())
		{
			ChangeState(KeenStates.Shoot);
		}
	}

	private void ChangeState(KeenStates newState)
	{
		if (Current.StateType == newState)
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
}
