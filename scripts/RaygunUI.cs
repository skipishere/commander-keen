using Godot;
using System;

public partial class RaygunUI : HBoxContainer
{
    private Label charges;

    private SignalManager signalManager;

    public override void _Ready()
    {
        charges = GetNode<Label>("Charges");
        signalManager = GetNode<SignalManager>("/root/SignalManager");
        //signalManager.ResetUi += ;
    }

    public override void _Process(double delta)
    {
        // Probably should make use of signal manager instead of directly accessing game_stats.
        charges.Text = game_stats.Charges.ToString("00");
    }
}
