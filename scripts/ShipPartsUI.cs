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
		
	}

	private void OnShipPart(game_stats.ShipParts shipPart)
    {
		Update(shipPart, true);
    }

	private void Update(game_stats.ShipParts shipPart, bool collected)
	{
		TextureRect part = shipPart switch
		{
			game_stats.ShipParts.Battery => battery,
			game_stats.ShipParts.JoyStick => joystick,
			game_stats.ShipParts.Vaccum => vaccum,
			game_stats.ShipParts.Fuel => fuel,
			_ => null
		};

		(part.Material as ShaderMaterial).SetShaderParameter("enable", !collected);
	}
}
