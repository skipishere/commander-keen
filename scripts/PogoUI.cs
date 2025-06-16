using Godot;

public partial class PogoUI : HBoxContainer
{
    private SignalManager signalManager;
    private TextureRect pogo;

    public override void _Ready()
    {
        signalManager = GetNode<SignalManager>("/root/SignalManager");
        pogo = GetNode<TextureRect>("Pogo");
        signalManager.PogoStick += OnPogoStick;
        signalManager.ResetUi += OnPogoStick;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        signalManager.PogoStick -= OnPogoStick;
        signalManager.ResetUi -= OnPogoStick;
    }

    private void OnPogoStick()
    {
        (pogo.Material as ShaderMaterial).SetShaderParameter("enable", game_stats.HasPogoStick == game_stats.PogoStickState.No);
    }
}
