using Godot;
using System;

public partial class KeyCardsUI : HBoxContainer
{
	private TextureRect cardA;
	private TextureRect cardB;
	private TextureRect cardC;
	private TextureRect cardD;
	private SignalManager signalManager;
	
	public override void _Ready()
	{
		cardA = GetNode<TextureRect>("A");
		cardB = GetNode<TextureRect>("B");
		cardC = GetNode<TextureRect>("C");
		cardD = GetNode<TextureRect>("D");

		signalManager = GetNode<SignalManager>("/root/SignalManager");
		signalManager.KeyCard += OnKeyCard;
		signalManager.ExitingLevel += OnResetUi;
	}

	public override void _ExitTree()
	{
		base._ExitTree();
		signalManager.KeyCard -= OnKeyCard;
		signalManager.ExitingLevel -= OnResetUi;
	}

    private void OnResetUi()
    {
		foreach(var color in Enum.GetValues(typeof(game_stats.KeyCards)))
		{
			OnKeyCard((game_stats.KeyCards)color, false);
		}
    }

    private void OnKeyCard(game_stats.KeyCards keyCard, bool collected)
    {
        TextureRect card = keyCard switch
		{
			game_stats.KeyCards.Yellow => cardA,
			game_stats.KeyCards.Red => cardB,
			game_stats.KeyCards.Green => cardC,
			game_stats.KeyCards.Blue => cardD,
			_ => null
		};
		
		(card.Material as ShaderMaterial).SetShaderParameter("enable", !collected);
    }
}
