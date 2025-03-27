using Godot;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;


namespace CommanderKeen.Scenes.Creatures.Vorticon;
public partial class VorticonStateMachine : Node
{
    public enum VorticonStates
	{
		Walk,
		Jump,
		Thinking,
		Dead
	}

    private readonly Dictionary<VorticonStates, IState<VorticonStates>> states = [];

    private IState<VorticonStates> Current { get; set; }

    [Export]
	private vorticon_guard character;

	[Export]
	private AnimationTree animationTree;

    private CharacterBody2D player;

    public override void _Ready()
	{
		foreach (VorticonBaseState state in GetChildren().OfType<VorticonBaseState>())
		{
			state.Character = character;
			state.AnimationTree = animationTree;
			states.Add(state.StateType, state);
			Debug.Print("Vorticon Added state: " + state.StateType);
		}
		
		Current = states.First().Value;
		Debug.Print("Vorticon Default state: " + Current.StateType);
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

	private void ChangeState(VorticonStates newState)
	{
		if (Current.StateType == newState)
		{
			return;
		}
		
		Debug.Print($"Vorticon State change - old: {Current.StateType}, New: {Current.NextState}");
		Current.Exit();
		Current = states[newState];
		Current.Enter();
	}

	public void TakeDamage()
	{
		character.Health--;
		if (character.Health <= 0)
		{
			Kill();
		}
	}

	public void Kill()
	{
		CallDeferred(nameof(ChangeState), (int)VorticonStates.Dead);
	}
}
