using Godot;
using System;

public partial class SignalManager : Node
{
	[Signal]
	public delegate void TeleportStartEventHandler();

	[Signal]
	public delegate void TeleportCompleteEventHandler(Vector2 position, bool finished);
	
	[Signal]
	public delegate void EnteringLevelEventHandler();

	[Signal]
	public delegate void KeenDeadEventHandler();
}
