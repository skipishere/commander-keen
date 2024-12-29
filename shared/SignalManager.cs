using Godot;
using System;

public partial class SignalManager : Node
{
	[Signal]
	public delegate void TeleportStartEventHandler();

	[Signal]
	public delegate void TeleportCompleteEventHandler(Vector2 position, bool finished);
	
	[Signal]
	public delegate void EnteringLevelEventHandler(string levelResource);

	[Signal]
	public delegate void ExitingLevelEventHandler();

	[Signal]
	public delegate void KeenDeadEventHandler();

	[Signal]
	public delegate void KeyCardEventHandler(game_stats.KeyCards keyCard, bool collected);

	[Signal]
	public delegate void ShipPartEventHandler(game_stats.ShipParts shipPart);

	[Signal]
	public delegate void ScoreChangedEventHandler();

	[Signal]
	public delegate void ResetUiEventHandler();

	[Signal]
	public delegate void ShowUiEventHandler(string title, string message);

	[Signal]
	public delegate void PogoStickEventHandler();

	[Signal]
	public delegate void HidePlayerEventHandler();
}
