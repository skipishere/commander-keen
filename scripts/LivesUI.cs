using Godot;

public partial class LivesUI : HBoxContainer
{
    private SignalManager signalManager;
    private Label lives;

    private AudioStreamPlayer audioPlayer;

    public override void _Ready()
    {
        lives = GetNode<Label>("Value");
        signalManager = GetNode<SignalManager>("/root/SignalManager");
        audioPlayer = GetNode<AudioStreamPlayer>("AudioStreamPlayer");

        signalManager.ResetUi += OnLivesChanged;
        signalManager.KeenDead += OnLivesChanged;
        signalManager.KeenGainedLife += OnKeenGainLife;
    }

    public override void _ExitTree()
    {
        base._ExitTree();
        signalManager.ResetUi -= OnLivesChanged;
        signalManager.KeenDead -= OnLivesChanged;
        signalManager.KeenGainedLife -= OnKeenGainLife;
    }

    private void OnKeenGainLife()
    {
        audioPlayer.Play();
        OnLivesChanged();
    }

    private void OnLivesChanged()
    {
        var livesToShow = game_stats.Lives > 0 ? game_stats.Lives : 0;
        lives.Text = livesToShow.ToString("00");
    }
}
