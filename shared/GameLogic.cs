using System;
using Godot;

[GlobalClass]
public partial class GameLogic : Node
{
    private SignalManager signalManager;

    private game_stats.ShipParts partsGained;
    private bool pogoStickGained;

    public GameLogic()
    {
    }

    public override void _EnterTree()
    {
        base._EnterTree();

        signalManager = GetNode<SignalManager>("/root/SignalManager");
        signalManager.KeenDead += OnKeenDead;
        signalManager.ExitingLevel += OnExitingLevel;
        signalManager.EnteringLevel += OnEnteringLevel;
    }

    private void OnEnteringLevel(string levelResource)
    {
        partsGained = game_stats.CollectedParts;
    }


    private void OnExitingLevel()
    {
        if (game_stats.HasPogoStick == game_stats.PogoStickState.Gained)
        {
            // Ensure Keen keeps the pogo stick if he finishes the level where he gained it.
            game_stats.HasPogoStick = game_stats.PogoStickState.Keep;
            signalManager.EmitSignal(nameof(SignalManager.PogoStick));
        }
    }

    public override void _ExitTree()
    {
        signalManager.KeenDead -= OnKeenDead;
        signalManager.ExitingLevel -= OnExitingLevel;
        signalManager.EnteringLevel -= OnEnteringLevel;
    }

    private void OnKeenDead()
    {
        game_stats.Lives--;

        // Reset collected parts if Keen dies.
        game_stats.CollectedParts = partsGained;
        signalManager.EmitSignal(nameof(SignalManager.ShipPart));

        if (game_stats.HasPogoStick == game_stats.PogoStickState.Gained)
        {
            // Reset pogo state if Keen dies and hasn't finished the level where he gained it.
            game_stats.HasPogoStick = game_stats.PogoStickState.No;
            signalManager.EmitSignal(nameof(SignalManager.PogoStick));
        }
    }
}