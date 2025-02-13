using Godot;
using System;

public partial class ShipPartsUI : HBoxContainer
{
	private TextureRect joystick;
	private TextureRect battery;
	private TextureRect vaccum;
	private TextureRect fuel;
	private SignalManager signalManager;

	public override void _Ready()
	{
		battery = GetNode<TextureRect>("Battery");
		joystick = GetNode<TextureRect>("Joystick");
		vaccum = GetNode<TextureRect>("Vaccum");
		fuel = GetNode<TextureRect>("Fuel");
		
		signalManager = GetNode<SignalManager>("/root/SignalManager");
		signalManager.ShipPart += OnShipPart;
		signalManager.ResetUi += OnShipPart;
	}

	private void OnShipPart()
    {
		foreach (game_stats.ShipParts part in Enum.GetValues(typeof(game_stats.ShipParts)))
		{
			TextureRect ui = part switch
			{
				game_stats.ShipParts.Battery => battery,
				game_stats.ShipParts.JoyStick => joystick,
				game_stats.ShipParts.Vaccum => vaccum,
				game_stats.ShipParts.Fuel => fuel,
				_ => null
			};

			(ui.Material as ShaderMaterial).SetShaderParameter("enable", !game_stats.CollectedParts.HasFlag(part));
		}
    }
}
