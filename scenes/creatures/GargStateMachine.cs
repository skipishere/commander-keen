using Godot;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

public partial class GargStateMachine : Node
{
    public enum GargStates
	{
		Walk,
		Agro,
		Thinking,
		Dead,
	}

    private readonly Dictionary<GargStates, IState<GargStates>> states = new();

    private IState<GargStates> Current { get; set; }

    [Export]
	private Garg character;

	[Export]
	private AnimationTree animationTree;

    private CharacterBody2D player;

	private bool isDead = false;

    public override void _Ready()
	{
		foreach (GargBaseState state in GetChildren().OfType<GargBaseState>())
		{
			state.player = character;
			state.AnimationTree = animationTree;
			states.Add(state.StateType, state);
			Debug.Print("Garg Added state: " + state.StateType);
		}
		
		Current = states.First().Value;
		Debug.Print("Garg Default state: " + Current.StateType);
	}

    public void PhysicsProcess(double delta, float lastMovementX, bool isActivated)
	{
		if (!isActivated 
			&& Current.StateType != GargStates.Agro)
		{
			return;
		}
		
		Current.PhysicsProcess(delta, lastMovementX);

		if (isDead)
		{
			ChangeState(GargStates.Dead);
		}
		else if (Current.NextState.HasValue)
		{
			Debug.Print($"Garg State change - old: {Current.StateType}, New: {Current.NextState}");
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
		isDead = true;
	}
}
